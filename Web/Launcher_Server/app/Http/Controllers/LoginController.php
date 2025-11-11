<?php

namespace App\Http\Controllers;

use App\Http\Requests\ApiLoginRequest;
use App\Http\Requests\MemberLoginRequest;
use App\Http\Requests\Validate2faRequest;
use App\Mailer;
use App\Models\Member;
use App\Models\MemberToken;
use App\Models\LoginTwoFactor as TwoFactor;
use Illuminate\Support\Facades\Cache;
use Illuminate\Support\Facades\Hash;

class LoginController extends \App\Http\BaseController
{
    const ACCOUNT_BANNED = 5;
    const LOGIN_WRONG_INFO = 0;
    const MAIL_SENT_FAILED = 6;
    const TOKEN_CREATION_FAILED = 9;
    const NEEDED_2FA_VERIFICATION = 2;
    const LOGIN_SUCCESSFUL = 1;
    const TOKEN_EXPIRED = 10;
    const MAIL_SENT_2FA_WAIT = 23;

    const LOGIN_HASH_CACHE_KEY_PREFIX = 'login-launcher-hash-';
    const LOGIN_HASH_EXPIRED = 11;
    const LOGIN_HASH_NOTFOUND = 15;
    const LOGIN_HASH_MAX_EXCESS = 19;
    const LOGIN_HASH_SUCCESSFUL = 21;

    private $currentMember;

    public function loginOld(MemberLoginRequest $request)
    {
        return $this->error('Launcher mới đã được cập nhật nhiều tính năng và nội dung chuyển tiền, vui lòng cập nhật lên phiên bản mới nhất!');
    }

    public function index(MemberLoginRequest $request)
    {
        $email = $request->username;
        $password = $request->password;
        $member = Member::where('Email', $email)
            ->first();
        if (!empty($member)) {
            if ($member->IsBan) {
                return $this->error('Tài khoản của bạn đã bị khóa!');
            }
			if (!Hash::check($password, $member->Password)) {
				return $this->error('Sai tài khoản hoặc mật khẩu!');
			}
            if (!empty($member->getAttribute('2fa')) && !empty($member->Fullname)) {
                return $this->twoFactorProcess($member);
            }
            //fix for error of over time
            $member->Email = strtolower($member->Email);
            $member->ActiveIP = $request->ip();
            $member->save();
            return $this->loginSuccess($member);
        }
        return $this->error('Sai tài khoản hoặc mật khẩu!');
    }

    private function twoFactorProcess($member, $onlyStatus = false)
    {
        $last2fa = TwoFactor::where('MemberID', $member->UserID)
            ->where('status', 0)
            ->orderBy('created_at', 'DESC')
            ->first();
        if (!empty($last2fa)) {
            $last_time = strtotime($last2fa->created_at);
            $time = strtotime('now') - $last_time;
            if ($time < env('2FA_WAITING_TIME_TO_RESEND', 60)) {
                $rest_time = env('2FA_WAITING_TIME_TO_RESEND', 60) - $time;
                if (!$onlyStatus) {
                    return $this->error('Hãy đợi ' . str_pad(intval($rest_time / 60), 2, '0', STR_PAD_LEFT) . ':' . str_pad(intval($rest_time % 60), 2, '0', STR_PAD_LEFT) . 's nữa rồi thử lại!');
                } else {
                    return self::MAIL_SENT_2FA_WAIT;
                }
            }
        }
        $mail_sent = $this->sendCode($member);
        if ($mail_sent['success']) {
            //shut all code down
            TwoFactor::where('MemberID', $member->UserID)
                ->where('status', 0)
                ->update(['status' => 2]);
            $twofa = new TwoFactor();
            $twofa->MemberID = $member->UserID;
            $twofa->code = md5($mail_sent['code']);
            $twofa->status = 0;
            if (!$twofa->save()) {
                if (!$onlyStatus) {
                    return $this->error('Không thể kết nối đến máy chủ xác thực 2 lớp, vui lòng thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
                } else {
                    return self::TOKEN_CREATION_FAILED;
                }
            }
            if (!$onlyStatus) {
                return $this->success(sensorEmail($member->Fullname), [], 2);
            } else {
                return self::NEEDED_2FA_VERIFICATION;
            }
        }
        if (!$onlyStatus) {
            return $this->error('Không thể gửi mã xác thực, thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
        } else {
            return self::MAIL_SENT_FAILED;
        }
    }

    /**
     * @param $member Member
     */
    private function createToken($member)
    {
        MemberToken::where('UserID', $member->UserID)->delete();
        $token = $member->createToken();
        $memberToken = new MemberToken();
        $memberToken->UserID = $member->UserID;
        $memberToken->token = $token;
        $memberToken->save();
        return $token;
    }

    private function getUserInfoForLogin($member)
    {
        return [
            'UserID' => $member->UserID,
            'UserName' => $member->Email,
            'Email' => sensorEmail($member->Fullname),
            'Money' => $member->Money,
            '2fa' => $member->getAttribute('2fa'),
            'VerifiedEmail' => $member->VerifiedEmail,
        ];
    }

    private function loginSuccess($member)
    {
        $token = $this->createToken($member);
        return $this->success('Đăng nhập thành công!',
            [
                'token' => $token,
                'userInfo' => $this->getUserInfoForLogin($member)
            ]
        );
    }

    private function sendCode($member)
    {
        $mailer = new Mailer();
        $mailer->setMember($member);
        $mailer->setTwoFactorInstance(new TwoFactor());
        return $mailer->TwoFactorAuthentication($member->Fullname);
    }

    public function validatetfa(Validate2faRequest $request)
    {
        $code = $request->code;
        $existed2facode = TwoFactor::where('code', md5($code))
            ->where('status', 0)
            ->orderBy('created_at', 'DESC')
            ->get();
        if (!empty($existed2facode) && $existed2facode->count()) {
            $last2fa = $existed2facode->first();
            $last_time = strtotime($last2fa->created_at);
            $time = strtotime('now') - $last_time;
            if ($time < env('2FA_CODE_EXPIRES', 120)) {
                $last2fa->status = 1;
                $member_id = $last2fa->MemberID;
                $member = Member::find($member_id);
                if (!empty($member) && $last2fa->save()) {
                    $member->setAttribute('2fa', 1);
                    $member->save();
                    return $this->loginSuccess($member);
                }
                return $this->error('Có lỗi xảy ra, vui lòng thử lại hoặc liên hệ quản trị viên để được trợ giúp!');
            }
            foreach ($existed2facode as $item) {
                $item->status = 2;
                $item->save();
            }
            return $this->error('Mã hết hiệu lực!');
        }
        return $this->error('Mã xác thực không tồn tại hoặc đã hết hiệu lực!');
    }

    /**
     * Login for new launcher
     */

    private function isBlockedAccess($username, $return = false)
    {
        $key = 'login-by-launcher-'.$username.'-attempts';
        $keyTime = 'login-by-launcher-'.$username.'-blocked';
        if (Cache::has($key)) {
            $number = Cache::get($key);
            if ($number > 10) {
                if (strtotime('now') - Cache::get($keyTime) > 30*1000) {
                    Cache::forget($key);
                    Cache::forget($keyTime);
                    return true;
                }
                if (!$return) {
                    $this->error('No Access!');
                }
                return false;
            }
            Cache::increment($key);
        } else {
            Cache::remember($key, 60*60, function () {
                return 1;
            });
            Cache::remember($keyTime, 60*60, function () {
                return strtotime('now');
            });
        }
        return true;
    }

    protected function getLogin($username, $password, $check2fa = true)
    {
        $existedMember = Member::where('Email', $username)
            ->where('Password', md5($password))
            ->get();
        if (!empty($existedMember) && $existedMember->count()) {
            $member = $existedMember->first();
            $this->currentMember = $member;
            if ($member->IsBan) {
                return self::ACCOUNT_BANNED;
            }
            if ($check2fa && !empty($member['2fa']) && $member->Fullname) {
                return $this->twoFactorProcess($member, true);
            }
            return self::LOGIN_SUCCESSFUL;
        }
        return self::LOGIN_WRONG_INFO;
    }

    public function login(ApiLoginRequest $request)
    {
        $username = $request->username;
        $password = $request->password;

        //$this->isBlockedAccess($username, true);

        $login_status = $this->getLogin($username, $password);
        switch ($login_status) {
            case self::ACCOUNT_BANNED:
                return $this->error('Tài khoản của bạn đã bị khóa!');
            case self::LOGIN_SUCCESSFUL:
                $serverList = \App\Models\ServerList::select('ServerID', 'ServerName')->get();
                return $this->success('Đăng nhập thành công!', [
                    'username' => $this->currentMember->Email,
                    'email' => $this->currentMember->Fullname,
                    'coin' => number_format($this->currentMember->Money),
                    'servers' => $serverList,
                ]);
            case self::NEEDED_2FA_VERIFICATION:
                return $this->success('Đăng nhập thành công!', [
                    'email' => sensorEmail($this->currentMember->Fullname),
                    'MemberID' => $this->currentMember->UserID,
                    //'expires' => env('2FA_LAUNCHER_TOKEN_EXPIRES', 7200),
                ], 2);
            case self::TOKEN_CREATION_FAILED:
            case self::MAIL_SENT_FAILED:
                return $this->error('Server gặp sự cố, vui lòng báo cáo cho quản trị viên để khắc phục!');
        }
        return $this->error('Sai tài khoản hoặc mật khẩu!');
    }
}

<?php

namespace App\Http\Controllers;

use App\Http\Requests\ApiLoginRequest;
use App\Http\Requests\Validate2faRequest;
use App\Mailer;
use App\Models\LoginTwoFactor as TwoFactor;
use App\Models\Member;
use App\Models\Player;
use App\Models\ServerList;
use Illuminate\Support\Facades\Hash;

class LauncherLiteController extends \App\Http\BaseController
{
    const ACCOUNT_BANNED = 5;
    const LOGIN_WRONG_INFO = -1;
    const MAIL_SENT_FAILED = 6;
    const TOKEN_CREATION_FAILED = 9;
    const NEEDED_2FA_VERIFICATION = 2;
    const LOGIN_SUCCESSFUL = 1;
    const TOKEN_EXPIRED = 10;
    const MAIL_SENT_2FA_WAIT = 23;

    public function loginGameOld()
    {
        return $this->error('Vui lòng tải launcher lite bản mới nhất ('.env('LAUNCHER_LITE_VERSION', '1.0.0.0').') để chơi game!');
    }

    protected function getLogin($username, $password, $check2fa = true, $ip = '')
    {
        $existedMember = Member::where('Email', $username)
            ->get();
        if (!empty($existedMember) && $existedMember->count()) {
            $member = $existedMember->first();
            if ($member->IsBan) {
                return self::ACCOUNT_BANNED;
            }
            if (!Hash::check($password, $member->Password)) {
                return $this->error('Sai tài khoản hoặc mật khẩu!');
            }
            $member->Email = strtolower($member->Email);
//            $member->ActiveIP = $ip;
            $member->save();
            //$member->login();
            if ($check2fa && !empty($member->getAttribute('2fa')) && !empty($member->Fullname)) {
                return $this->twoFactorProcess($member, false);
            }
            $this->currentMember = $member;
            return self::LOGIN_SUCCESSFUL;
        }
        return self::LOGIN_WRONG_INFO;
    }

    public function loginGame(ApiLoginRequest $request)
    {
        $username = strtolower($request->input('username'));
        $password = $request->input('password');

        //$this->isBlockedAccess($username);

        $login_status = $this->getLogin($username, $password, true, $request->ip());
        if ($login_status === self::ACCOUNT_BANNED) {
            return $this->error('Tài khoản của bạn đã bị khóa!');
        }
        if ($login_status === self::LOGIN_WRONG_INFO) {
            return $this->error('Sai tài khoản hoặc mật khẩu!');
        }
        if ($login_status !== self::LOGIN_SUCCESSFUL) {
            return $login_status;
        }
        return $this->loginSuccess($this->currentMember);
    }

    /**
     * Ref LoginController::twoFactorProcess()
     * @param $member
     * @param false $onlyStatus
     * @return \Illuminate\Http\JsonResponse|int
     */
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
                return $this->success(sensorEmail($member->Fullname), [
                    'PlayUrl' => '',
                    'TwoFactor' => true
                ], true);
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
     * @param Member $member
     * @return \Illuminate\Http\JsonResponse
     */
    private function loginSuccess($member)
    {
        $svid = 1001;
        $bypass = false;
        $serverInfo = ServerList::find($svid);
        if (empty($serverInfo)) {
            return $this->error('Server Die!');
        }
        $keyrand = rand(111111, 999999);
        $timeNow = time();
        $requestLink = $serverInfo->LinkRequest;
        $content = file_get_contents( trim($requestLink, '/'). "/CreateLogin.aspx?content=" . $member->Email . "|" . strtoupper($keyrand) . "|" . $timeNow . "|" . md5($member->Email . strtoupper($keyrand) . $timeNow . env('KEY_REQUEST', 'no-one_is_promised_tomorow')));
        if (trim($content) != "0") {
            return $this->error($content);
        }
        $configLink = '';
        $flashFile = 'Loading.swf';
        $flashUrl = '';
//        $player = Player::select('UserID', 'LoginDevice')->where('UserName', $member->Email)->first();
//        if (!empty($player)) {
//            $player->LauncherLoggedin();
            $configLink = $serverInfo->LinkConfig;
            $flashUrl = trim($serverInfo->LinkFlash, '/');
//        } else {
//            $configLink = $serverInfo->LinkConfigRegister;
//            $flashUrl = trim(env('FLASH_URL_REGISTER'), '/');
//        }
        $text = $flashUrl.'/Loading.swf?user=' . $member->Email . '&key=' . $keyrand . '&v=10950&rand=' . rand(100000000, 999999999) . '&config=' . $configLink . '&sessionId=' . rand(100000000, 999999999);//dang nhap thanh cong
        return $this->success('', ['playUrl' => $text]);
    }
}

<?php

namespace App\Http\Controllers;

use App\Http\Requests\ChangePasswordRequest;
use App\Http\Requests\Validate2faExchangeRequest;
use App\Http\Requests\Validate2faRequest;
use App\Mailer;
use App\Models\ChangePasswordTwoFactor as TwoFactor;
use App\Models\ExchangeTwoFactor;
use App\Models\Member;
use App\Models\MemberToken;
use App\Models\Player;
use App\Models\ServerList;

class ChangePasswordController extends \App\Http\BaseController
{
    public function index(ChangePasswordRequest $request)
    {
        $current_password = $request->current_password;
        /** @var Member $member */
        $member = auth()->user();
        if (md5($current_password) == $member->Password) {
            if (!empty($member->getAttribute('Fullname')) && !empty($member->getAttribute('VerifiedEmail'))) {
                $last2fa = TwoFactor::where('MemberID', $member->UserID)
                    ->where('status', 0)
                    ->orderBy('created_at', 'DESC')
                    ->first();
                if (!empty($last2fa)) {
                    $last_time = strtotime($last2fa->created_at);
                    $time = strtotime('now') - $last_time;
                    if ($time < env('2FA_WAITING_TIME_TO_RESEND', 60)) {
                        $rest_time = env('2FA_WAITING_TIME_TO_RESEND', 60) - $time;
                        return $this->error('Hãy đợi ' . str_pad(intval($rest_time/60), 2, '0', STR_PAD_LEFT).':'.str_pad(intval($rest_time%60), 2, '0', STR_PAD_LEFT).'s nữa rồi thử lại!');
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
                    $twofa->data = json_encode([
                        'new_password' => $request->new_password
                    ]);
                    if (!$twofa->save()) {
                        return $this->error('Không thể kết nối đến máy chủ xác thực 2 lớp, vui lòng thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
                    }
                    return $this->success(sensorEmail($member->Fullname), [], 2);
                }
                return $this->error('Không thể gửi mã xác thực, thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
            }
            $member->Password = md5($request->new_password);
            if($member->save()) {
                MemberToken::where('UserID', $member->UserID)->delete();
                return $this->success("Đổi mật khẩu thành công!");
            }
        } else {
            return $this->error("Mật khẩu cũ không đúng!");
        }
        return $this->error("Không thể đổi mật khẩu vào lúc này, vui lòng thử lại sau!");
    }

    private function sendCode($member)
    {
        $mailer = new Mailer();
        $mailer->setMember($member);
        $mailer->setTwoFactorInstance(new TwoFactor());
        return $mailer->ChangePass($member->Fullname);
    }

    public function validatetfa(Validate2faRequest $request)
    {
        $code = $request->code;
        $member = auth()->user();
        $this->member = $member;
        $existed2facode = TwoFactor::where('MemberID', $member->UserID)
            ->where('code', md5($code))
            ->where('status', 0)
            ->orderBy('created_at', 'DESC')
            ->get();
        if (!empty($existed2facode) && $existed2facode->count()) {
            $last2fa = $existed2facode->first();
            $last_time = strtotime($last2fa->created_at);
            $time = strtotime('now') - $last_time;
            if ($time < env('2FA_CODE_EXPIRES', 120)) {
                $last2fa->status = 1;
                if ($last2fa->save()) {
                    $changePassData = json_decode($last2fa->getAttribute('data'), true);
                    if (empty($changePassData['new_password'])) {
                        return $this->error('Không có dữ liệu đổi mật khẩu, vui lòng liên hệ quản trị viên để được trợ giúp!');
                    }
                    $member->Password = md5($changePassData['new_password']);
                    if($member->save()) {
                        return $this->success("Đổi mật khẩu thành công!");
                    }
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
}

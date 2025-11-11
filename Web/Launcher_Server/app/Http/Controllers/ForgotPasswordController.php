<?php

namespace App\Http\Controllers;

use App\Http\Requests\ForgotPasswordRequest;
use App\Http\Requests\Validate2faExchangeRequest;
use App\Http\Requests\Validate2faRequest;
use App\Mailer;
use App\Models\ForgotPasswordAttemps;
use App\Models\ExchangeTwoFactor;
use App\Models\Member;
use App\Models\Player;
use App\Models\ServerList;
use Illuminate\Support\Str;

class ForgotPasswordController extends \App\Http\BaseController
{
    public function index(ForgotPasswordRequest $request)
    {
        $account = $request->account;
        /** @var Member $member */
        $member = Member::where('Email', $account)->orWhere('Fullname', $account)->first();
        if (empty($member)) {
            return $this->error('Không tìm thấy tài khoản đăng nhập hoặc email');
        }
        $lastToken = ForgotPasswordAttemps::where('MemberID', $member->UserID)
            ->where('status', 0)
            ->orderBy('created_at', 'DESC')
            ->first();
        if (!empty($lastToken)) {
            $last_time = strtotime($lastToken->created_at);
            $time = strtotime('now') - $last_time;
            if ($time < env('2FA_WAITING_TIME_TO_RESEND', 60)) {
                $rest_time = env('2FA_WAITING_TIME_TO_RESEND', 60) - $time;
                return $this->error('Hãy đợi ' . str_pad(intval($rest_time/60), 2, '0', STR_PAD_LEFT).':'.str_pad(intval($rest_time%60), 2, '0', STR_PAD_LEFT).'s nữa rồi thử lại!');
            }
        }
        $mail_sent = $this->sendCode($member);
        if ($mail_sent['success']) {
            //shut all code down
            ForgotPasswordAttemps::where('MemberID', $member->UserID)
                ->where('status', 0)
                ->update(['status' => 2]);
            $attemp = new ForgotPasswordAttemps();
            $attemp->MemberID = $member->UserID;
            $attemp->code = md5($mail_sent['code']);
            $attemp->status = 0;
            if (!$attemp->save()) {
                return $this->error('Không thể kết nối đến máy chủ, vui lòng thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
            }
            return $this->success(sensorEmail($member->Fullname), [], 2);
        }
        return $this->error('Không thể gửi mã xác thực, thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
    }

    private function sendCode($member)
    {
        $mailer = new Mailer();
        $mailer->setMember($member);
        $mailer->setTwoFactorInstance(new ForgotPasswordAttemps());
        return $mailer->ForgotPass($member->Fullname);
    }

    public function validate(Validate2faRequest $request)
    {
        $code = $request->code;
        $existed2facode = ForgotPasswordAttemps::where('code', md5($code))
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
                    $member = Member::find($last2fa->MemberID);
                    if (empty($member)) {
                        return $this->error('Có lỗi xảy ra, vui lòng thử lại hoặc liên hệ quản trị viên để được trợ giúp!');
                    }
                    $newpass = Str::random(10);
                    $member->Password = md5($newpass);
                    $mail_sent = $this->sendPassword($member, $newpass);
                    if($mail_sent && $member->save()) {
                        return $this->success("Khôi phục mật khẩu thành công, mật khẩu mới đã được gửi đến email đăng ký!");
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

    private function sendPassword($member, $newpass)
    {
        $mailer = new Mailer();
        $mailer->setMember($member);
        $mailer->setTwoFactorInstance(new ForgotPasswordAttemps());
        return $mailer->NewPass($member->Fullname, $newpass);
    }
}

<?php

namespace App\Http\Controllers;

use App\Http\Requests\ChangeEmailRequest;
use App\Http\Requests\Validate2faRequest;
use App\Mailer;
use App\Models\ChangeEmailTwoFactor as TwoFactor;
use App\Models\Member;

class ChangeEmailController extends \App\Http\BaseController
{
    public function index(ChangeEmailRequest $request)
    {
        $email = $request->email;
        /** @var Member $member */
        $member = auth()->user();
        if (!empty($member->getAttribute('Fullname'))) {
            if ($member->getAttribute('Fullname') == $email) {
                return $this->error('Vui lòng sử dụng email khác với email hiện tại!');
            }
            $existed = Member::where('Fullname', $email)->first();
            if (!empty($existed)) {
                return $this->error('Email đã có người sử dụng, vui lòng chọn email khác!');
            }
            if (!empty($member->getAttribute('VerifiedEmail'))) {
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
                $mail_sent = $this->sendCode($member, $email);
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
                        'new_email' => $email
                    ]);
                    if (!$twofa->save()) {
                        return $this->error('Không thể kết nối đến máy chủ xác thực 2 lớp, vui lòng thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
                    }
                    return $this->success(sensorEmail($member->Fullname), [], 2);
                }
                return $this->error('Không thể gửi mã xác thực, thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
            }
        }
        $member->Fullname = $email;
        if($member->save()) {
            return $this->success("Đổi email thành công!");
        }
        return $this->error("Không thể đổi mật khẩu vào lúc này, vui lòng thử lại sau!");
    }

    private function sendCode($member, $newEmail)
    {
        $mailer = new Mailer();
        $mailer->setMember($member);
        $mailer->setTwoFactorInstance(new TwoFactor());
        return $mailer->ChangeEmail($member->Fullname, $newEmail);
    }

    private function sendCodeConfirm($member, $newEmail)
    {
        $mailer = new Mailer();
        $mailer->setMember($member);
        $mailer->setTwoFactorInstance(new TwoFactor());
        return $mailer->ChangeEmailConfirm($newEmail, $member->Fullname);
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
                $changeEmailData = json_decode($last2fa->getAttribute('data'), true);
                if (empty($changeEmailData['new_email'])) {
                    return $this->error('Không có dữ liệu đổi email, vui lòng liên hệ quản trị viên để được trợ giúp!');
                }
                $mail_sent = $this->sendCodeConfirm($member, $changeEmailData['new_email']);
                if ($mail_sent['success']) {
                    $last2fa->status = 3;
                    $last2fa->code = md5($mail_sent['code']);
                    if (!$last2fa->save()) {
                        return $this->error('Không thể kết nối đến máy chủ xác thực 2 lớp, vui lòng thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
                    }
                    return $this->success();
                }
                return $this->error('Không thể gửi mail xác nhận đến địa chỉ email mới, vui lòng thử lại hoặc liên hệ quản trị viên để được trợ giúp!');
            }
            foreach ($existed2facode as $item) {
                $item->status = 2;
                $item->save();
            }
            return $this->error('Mã hết hiệu lực!');
        }
        return $this->error('Mã xác thực không tồn tại hoặc đã hết hiệu lực!');
    }

    public function confirmvalidatetfa(Validate2faRequest $request)
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
            $last_time = strtotime($last2fa->updated_at);
            $time = strtotime('now') - $last_time;
            if ($time < env('2FA_CODE_EXPIRES', 120)) {
                $last2fa->status = 1;
                if ($last2fa->save()) {
                    $changeEmailData = json_decode($last2fa->getAttribute('data'), true);
                    if (empty($changeEmailData['new_email'])) {
                        return $this->error('Không có dữ liệu đổi email, vui lòng liên hệ quản trị viên để được trợ giúp!');
                    }
                    $member->Fullname = $changeEmailData['new_email'];
                    $member->VerifiedEmail = 0;
                    $member->setAttribute('2fa', 0);
                    if($member->save()) {
                        return $this->success("Đổi email thành công! Xác thực lại email ngay nhé bạn!", ['Email' => sensorEmail($member->Fullname)]);
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

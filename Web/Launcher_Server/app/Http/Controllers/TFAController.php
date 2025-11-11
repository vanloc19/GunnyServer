<?php

namespace App\Http\Controllers;

//use App\ForgotPassAttempt;
use App\Http\Requests\Validate2faRequest;
use App\Mailer;
use App\Models\Member;
use App\Models\ToggleTwoFactor as TwoFactor;
use App\Models\VerifyEmail;

class TFAController extends \App\Http\BaseController
{
    public function verifyEmail()
    {
        $member = auth()->user();
        if (empty($member->Fullname)) {
            return $this->error('Bạn chưa đăng ký email, vui lòng đổi email để thực hiện xác thực!');
        }
        $last2fa = VerifyEmail::where('MemberID', $member->UserID)
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
        $mailer = new Mailer();
        $mailer->setMember($member);
        $mailer->setTwoFactorInstance(new VerifyEmail());
        $mail_sent = $mailer->verifyEmail($member->Fullname);
        if ($mail_sent['success']) {
            //shut all code down
            VerifyEmail::where('MemberID', $member->UserID)
                ->where('status', 0)
                ->update(['status' => 2]);
            $twofa = new VerifyEmail();
            $twofa->MemberID = $member->UserID;
            $twofa->code = md5($mail_sent['code']);
            $twofa->status = 0;
            if (!$twofa->save()) {
                return $this->error('Không thể kết nối đến máy chủ email, vui lòng thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
            }
            return $this->success('Gửi mã thành công, nhập mã xác thực!', []);
        } else {
            return $this->error('Không thể gửi email xác thực lúc này, vui lòng thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
        }
        return $this->success('Thành công');
    }

    public function validateVerifyEmail(Validate2faRequest $request)
    {
        $member = auth()->user();
        $code = $request->code;
        $existed2facode = VerifyEmail::where('MemberID', $member->UserID)
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
                    $member->setAttribute('VerifiedEmail', 1);
                    $member->save();
                    return $this->success('Xác thực email thành công!');
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

    public function active()
    {
        $member = auth()->user();
        if (empty($member->Fullname)) {
            return $this->error('Bạn chưa đăng ký email, vui lòng đổi email để thực hiện kích hoạt xác thực 2 lớp!');
        }
        if (!$member->VerifiedEmail) {
            return $this->error('Bạn chưa xác thực email, vui lòng sử dụng launcher bản mới nhất để xác thực email!');
        }
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
        $mailer = new Mailer();
        $mailer->setMember($member);
        $mailer->setTwoFactorInstance(new TwoFactor());
        $mail_sent = $mailer->active2fa($member->Fullname);
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
                return $this->error('Không thể kết nối đến máy chủ xác thực 2 lớp, vui lòng thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
            }
            return $this->success('Kích hoạt thành công, nhập mã xác thực!', []);
        } else {
            return $this->error('Không thể gửi email xác thực lúc này, vui lòng thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
        }
        return $this->success('Thành công');
    }

    public function validate(Validate2faRequest $request)
    {
        $member = auth()->user();
        $code = $request->code;
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
                    $member->setAttribute('2fa', 1);
                    $member->save();
                    return $this->success('Kích hoạt xác thực 2 lớp thành công!');
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

    public function deactive()
    {
        $member = auth()->user();
        if (empty($member->Fullname) || empty($member->getAttribute('2fa'))) {
            return $this->error('Bạn chưa kích hoạt xác thực 2 lớp!');
        }
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
        $mailer = new Mailer();
        $mailer->setMember($member);
        $mailer->setTwoFactorInstance(new TwoFactor());
        $mail_sent = $mailer->deactive2fa($member->Fullname);
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
                return $this->error('Không thể kết nối đến máy chủ xác thực 2 lớp, vui lòng thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
            }
        } else {
            return $this->error('Không thể gửi email xác thực lúc này, vui lòng thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
        }
        return $this->success('Thành công');
    }

    public function validateDeactive(Validate2faRequest $request)
    {
        $member = auth()->user();
        $code = $request->code;
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
                    $member->setAttribute('2fa', 0);
                    $member->save();
                    return $this->success('Ngừng kích hoạt xác thực 2 lớp thành công!');
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

    /*
    protected function sendCode($type = 'twofactor')
    {
        $mailer = new Mailer();
        if ($type == 'twofactor') {
            $twofa = new TwoFactor();
            $member_id = $this->getSession('2fa_member_id');
            $member = Member::findOrFail($member_id);
            $mail_sent = $mailer->TwoFactorAuthentication($member->Fullname);
        } else if ($type == 'forgotpass') {
            $twofa = new ForgotPassAttempt();
            $member_id = $this->getSession('forgotpass_member_id');
            $member = Member::findOrFail($member_id);
            $mail_sent = $mailer->ForgotPass($member->Fullname);
        }
        if ($mail_sent['success']) {
            $twofa->MemberID = $member->UserID;
            $twofa->code = md5($mail_sent['code']);
            $twofa->status = 0;
            if (!$twofa->save()) {
                return 1; //error when save
            }
            return 0; //success
        }
        return 2; //cannot send mail
    }
    */
}

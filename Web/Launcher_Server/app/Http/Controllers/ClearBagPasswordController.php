<?php

namespace App\Http\Controllers;

use App\Http\Requests\BagPasswordRequest;
use App\Http\Requests\Validate2faRequest;
use App\Mailer;
use App\Models\ClearBagPasswordCode as TwoFactor;
use App\Models\ExchangeTwoFactor;
use App\Models\Member;
use App\Models\MemberToken;
use App\Models\Player;
use App\Models\PlayerPasswordSecurity;
use App\Models\ServerList;
use Illuminate\Support\Facades\DB;

class ClearBagPasswordController extends \App\Http\BaseController
{
    public function index(BagPasswordRequest $request)
    {
        $server_id = $request->server_id;
        $player_id = $request->player_id;
        if (empty($server_id) || empty($player_id)) {
            return $this->error('Vui lòng nhập đẩy đủ thông tin!');
        }
        /** @var Member $member */
        $member = auth()->user();
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
                $twofa->server_id = $server_id;
                $twofa->player_id = $player_id;
                if (!$twofa->save()) {
                    return $this->error('Không thể kết nối đến máy chủ xác thực, vui lòng thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
                }
                return $this->success(sensorEmail($member->Fullname), [], 2);
            }
            return $this->error('Không thể gửi mã xác thực, thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
        }
        return $this->error("Tài khoản của bạn chưa có email hoặc chưa xác thực email, vui lòng xác thực email trước khi thực hiện hành động này!");
    }

    private function sendCode($member)
    {
        $mailer = new Mailer();
        $mailer->setMember($member);
        $mailer->setTwoFactorInstance(new TwoFactor());
        return $mailer->ClearBagPass($member->Fullname);
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
                    $server = ServerList::where('ServerID', $last2fa->server_id)->first();
                    if (empty($server)) {
                        return $this->error('Dữ liệu không hợp lệ, vui lòng thử lại hoặc liên hệ quản trị viên để được trợ giúp!');
                    }
                    $player = Player::on($server->Connection)->select('UserID')
                        ->where('UserID', $last2fa->player_id)
                        ->first();
                    if (empty($player)) {
                        return $this->error('Dữ liệu không hợp lệ, vui lòng thử lại hoặc liên hệ quản trị viên để được trợ giúp!');
                    }
                    DB::connection($server->Connection)->beginTransaction();
                    if (Player::on($server->Connection)
                            ->where('UserID', $player->UserID)
                            ->update(['PasswordTwo' => null]) &&
                        PlayerPasswordSecurity::where('UserID', $player->UserID)
                            ->delete()
                    ) {
                        DB::connection($server->Connection)->commit();
                        return $this->success("Xóa mật khẩu rương và câu hỏi bảo mật thành công!");
                    } else {
                        DB::connection($server->Connection)->rollBack();
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

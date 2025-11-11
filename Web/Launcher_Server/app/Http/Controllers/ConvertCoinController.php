<?php

namespace App\Http\Controllers;

use App\Http\Requests\ConvertCoinRequest;
use App\Http\Requests\Validate2faExchangeRequest;
use App\Mailer;
use App\Models\ExchangeTwoFactor;
use App\Models\Member;
use App\Models\MemberHistory;
use App\Models\LogCardCham;
use App\Models\Player;
use App\Models\ServerList;
use App\Models\Setting;
use App\Models\TwoFactor;
use Illuminate\Contracts\Cache\LockTimeoutException;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\Cache;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Log;

class ConvertCoinController extends \App\Http\BaseController
{
    protected $member;
    public function index(ConvertCoinRequest $request)
    {
        $playerId = $request->input('player_id');
        $lock = Cache::lock('convert-coin-for-'.$playerId, 10);
        $status = false;
        try {
             $lock->get(function () use ($request, $playerId, &$status) {
                $member = auth()->user();
                $this->member = $member;
                if ($member->IsBan == 1) {
                    return $this->error('Tài khoản của bạn đã bị khoá');
                }

                $coin = $request->input('coin');
                $coin = intval($coin);
                $coin = max(600, $coin);
                $coin = min(1000000, $coin);
                //checkTotal charged
                $checkTotalCharge = $this->checkTotalCharge($coin);
                if ($checkTotalCharge !== true) {
                    $status = $checkTotalCharge;
                    return false;
                }

                $serverId = $request->input('server_id');
                $server = ServerList::find($serverId);
                if (empty($server)) {
                    $status = $this->error('Server không tồn tại');
                    return false;
                }
                $serverConnection = $server->Connection;

                $player = Player::on($serverConnection)->select('UserID', 'NickName')
                    ->where('UserName', $member->Email)
                    ->where('UserID', $playerId)
                    ->first();
                if (empty($player)) {
                    $status =  $this->error('Nhân vật không tồn tại!');
                    return false;
                }
                if (!empty($member->getAttribute('2fa')) && !empty($member->Fullname)) {
                    $status = $this->sendCode($serverId, $playerId, $coin);
                    return false;
                }
                 $status = $this->charge($server, $player, $coin, $request->header('x-real-ip'));
                 return false;
            });
        } catch (LockTimeoutException $e) {
            return $this->error('Thao tác quá nhanh', [], 403);
        } finally {
            optional($lock)->release();
        }
        if ($status === true) {
            return $this->success('Chuyển xu thành công');
        } elseif ($status === false) {
            return $this->error('Thao tác thất bại');
        }
        return $status;
    }

    private function checkTotalCharge($coin)
    {
        //Get total charged for checking
        $memHistory = new MemberHistory();
        $totalChanged = $memHistory->memberTotalChargedToday($this->member->UserID);
        $totalChangeAllowed = Setting::get('gioi-han-chuyen-xu-ngay');
        if ($this->member->Money < $coin)
            return $this->error('Không đủ coin để chuyển đổi!');

        if ($totalChanged > 0 && !empty($totalChangeAllowed) && intval($totalChangeAllowed) <= ($totalChanged + $coin)) {
            if ($totalChanged < intval($totalChangeAllowed))
                return $this->error('Số xu tối đa bạn có thể đổi trong hôm nay là '.(intval($totalChangeAllowed) - $totalChanged));
            return $this->error('Bạn đã đạt tối đa số xu nạp cho phép trong hôm nay, hãy quay lại vào ngày mai!');
        }
        return true;
    }

    private function charge($server, $player, $coin, $ip = '')
    {
        $member = $this->member;
        DB::connection('sqlsrv_mem')->beginTransaction();
        $memHistory = new MemberHistory();
        if(
//            $memHistory->memberChargeMoneyLog($member->UserID, $coin, $server->ServerName, $request->ip()) && //Without AntiDDos.vn
            $memHistory->memberChargeMoneyLog($member->UserID, $coin, $server->ServerName, $ip) &&
            $member->chargeMoney($coin)
        ){
            $chargeID = md5(uniqid());

            $options = array(
                'http'=>array(
                    'header'=>"User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) 37abc/2.0.6.16 Chrome/60.0.3112.113 Safari/537.36" // i.e. An iPad
                )
            );
            $context = stream_context_create($options);
            $content = file_get_contents(
                trim($server->LinkRequest, '/')
                . "/ChargeMoney.aspx?content="
                . $chargeID
                . "|" . $member->Email
                . "|" . ($coin * Setting::get('he-so-doi-coin'))
                . "|0" //payway
                . "|.00" //needMoney
                . "|" . md5($chargeID.$member->Email.($coin * Setting::get('he-so-doi-coin')).'0'.'.00'.env('CHARGE_KEY'))
                . "&nickname=" . $player->UserID
                ,false, $context);
            if (substr($content, 0, 1) === "0") {
                DB::connection('sqlsrv_mem')->commit();
                return $this->success('Chuyển xu thành công, nhận xu trong game nhé!', ['coin' => $member->Money]);

            } else {
                Log::error('Charge to '.$member->Email.' Error: '.$content);
                DB::connection('sqlsrv_mem')->rollBack();
                return $this->error('Không thể nạp xu vào game, vui lòng thoát game và thử lại hoặc thông báo tới quản trị viên để được trợ giúp!');
            }
        }
    }

    private function sendCode($serverId, $playerId, $coin)
    {
        $member = $this->member;
        $last2fa = ExchangeTwoFactor::where('MemberID', $member->UserID)
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
        $mailer->setTwoFactorInstance(new ExchangeTwoFactor());
        $mail_sent = $mailer->Exchange($member->Fullname, $coin);
        if ($mail_sent['success']) {
            //shut all code down
            ExchangeTwoFactor::where('MemberID', $member->UserID)
                ->where('status', 0)
                ->update(['status' => 2]);
            $twofa = new ExchangeTwoFactor();
            $twofa->MemberID = $member->UserID;
            $twofa->code = md5($mail_sent['code']);
            $twofa->status = 0;
            $twofa->data = json_encode([
                'server_id' => $serverId,
                'player_id' => $playerId,
                'coin' => $coin
            ]);
            if (!$twofa->save()) {
                return $this->error('Không thể kết nối đến máy chủ xác thực 2 lớp, vui lòng thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
            }
            return $this->success(sensorEmail($member->Fullname), [], 2);
        }
        return $this->error('Không thể gửi mã xác thực, thử lại sau hoặc liên hệ quản trị viên để được trợ giúp!');
    }

    public function validatetfa(Validate2faExchangeRequest $request)
    {
        $code = $request->code;
        $lock = Cache::lock('convert2fa-coin-for-'.$code, 10);
        $status = false;
        try {
            $lock->get(function () use ($request, $code, &$status) {
                $member = auth()->user();
                $this->member = $member;
                $existed2facode = ExchangeTwoFactor::where('MemberID', $member->UserID)
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
                            $exhangeData = json_decode($last2fa->getAttribute('data'), true);
                            if (empty($exhangeData['coin']) || empty($exhangeData['server_id']) || empty($exhangeData['player_id'])) {
                                $status = $this->error('Không có dữ liệu chuyển xu, vui lòng liên hệ quản trị viên để được trợ giúp!');
                                return false;
                            }
                            $coin = $exhangeData['coin'];
                            //checkTotal charged
                            $checkTotalCharge = $this->checkTotalCharge($coin);
                            if ($checkTotalCharge !== true) {
                                $status = $checkTotalCharge;
                                return false;
                            }
                            $serverId = $exhangeData['server_id'];
                            $server = ServerList::find($serverId);
                            if (empty($server)) {
                                $status = $this->error('Server không tồn tại');
                                return false;
                            }
                            $serverConnection = $server->Connection;
                            $playerId = $exhangeData['player_id'];
                            $player = Player::on($serverConnection)->select('UserID', 'NickName')
                                ->where('UserName', $member->Email)
                                ->where('UserID', $playerId)
                                ->first();
                            if (empty($player)) {
                                $status = $this->error('Nhân vật không tồn tại!');
                                return false;
                            }
                            $status = $this->charge($server, $player, $coin, $request->header('x-real-ip'));
                            return false;
                        }
                        $status = $this->error('Có lỗi xảy ra, vui lòng thử lại hoặc liên hệ quản trị viên để được trợ giúp!');
                        return false;
                    }
                    foreach ($existed2facode as $item) {
                        $item->status = 2;
                        $item->save();
                    }
                    $status = $this->error('Mã hết hiệu lực!');
                    return false;
                }
                $status = $this->error('Mã xác thực không tồn tại hoặc đã hết hiệu lực!');
                return false;
            });
        } catch (LockTimeoutException $e) {
            return $this->error('Thao tác quá nhanh', [], 403);
        } finally {
            optional($lock)->release();
        }
        if ($status === true) {
            return $this->success('chuyển xu thành công');
        } elseif ($status === false) {
            return $this->error('Thao tác thất bại');
        }
        return $status;
    }
}

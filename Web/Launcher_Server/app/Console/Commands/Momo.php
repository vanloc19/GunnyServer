<?php

namespace App\Console\Commands;

use App\Models\Member;
use App\Models\MemberHistory;
use App\Models\PaymentMomoHistory;
use App\Models\Setting;
use Illuminate\Console\Command;
use Illuminate\Support\Facades\Cache;
use Illuminate\Support\Facades\DB;

class Momo extends Command
{
    /**
     * The name and signature of the console command.
     *
     * @var string
     */
    protected $signature = 'momopayment';

    /**
     * The console command description.
     *
     * @var string
     */
    protected $description = 'Fetch momo payment to add coin every 30s';

    /**
     * Create a new command instance.
     *
     * @return void
     */
    public function __construct()
    {
        parent::__construct();
    }

    /**
     * Execute the console command.
     *
     * @return int
     */
    public function handle()
    {
        $this->rechargeMomo();
        return 0;
    }

    public function rechargeMomo()
    {
        $transList = $this->getMomoHistories();
        $isDirty = false;
        if (!empty($transList)) {
            $transInDb = Cache::rememberForever('momo_histories', function () {
                return PaymentMomoHistory::all();
            });
            foreach ($transList as $transaction) {
                $existedTrans = $transInDb->where('tranId', $transaction['tranId'])->first();
                if (!empty($existedTrans)) {
                    continue;
                }
                $status = $transaction['status'];
                $isTelegramAlert = false;
                if ($status == 999) {
                    $comment = (empty($transaction['comment']) || is_array($transaction['comment'])) ? '' : $transaction['comment'];
                    if (!empty($comment)) {
                        $cmt_parts = preg_match(getChargeRegex(), $comment, $matches);
                        if (!empty($matches[0])) {
                            $account = str_ireplace(env('CHARGE_PREFIX', 'EZG'), "", $matches[0]);
                            if (!empty($account)) {
                                $member = Member::where('UserID', (int)$account)->first();
                                if (!empty($member)) {
                                    $amount = $transaction['amount'];
                                    if ($member->getTotalCharged() > 0) {
                                        $member->Money += floor($amount * Setting::get('he-so-atm') * (100 + intval(Setting::get('khuyen-mai-nap-coin'))) / 100);
                                    } else {
                                        $member->Money += floor($amount * Setting::get('he-so-atm') * (100 + intval(Setting::get('khuyen-mai-nap-lan-dau'))) / 100);
                                    }
                                    DB::connection($member->getConnectionName())->beginTransaction();
                                    if (!$member->save()) {
                                        DB::connection($member->getConnectionName())->rollBack();
                                        continue;
                                    }
                                    $memHistory = new MemberHistory();
                                    $memHistory->memberReChargeMoneyLog($member->UserID, $amount, 1, '');
                                    \Telegram::sendMessage([
                                        'chat_id' => env('TELEGRAM_CHAT_ID', '-1'), 
                                        'text' => "\xE2\x9C\x85\[".date('Y-m-d H:i:s')."]Member *" . $member->Email. "* đã nạp *" . $amount. "* qua *Momo*\n*Nội dung:* ".$comment,
                                        'parse_mode' => "Markdown"
                                    ]);
                                    $isTelegramAlert = true;
                                }
                            }
                        }
                    }
                    $data = [
                        'comment' => $comment,
                        'tranId' => $transaction['tranId'],
                        'amount' => $transaction['amount'],
                        'user' => $transaction['user'],
                        'partnerId' => $transaction['partnerId'],
                        'partnerName' => $transaction['partnerName'],
                        'status' => $status,
                    ];
                    $payment = new PaymentMomoHistory($data);
                    DB::connection($payment->getConnectionName())->beginTransaction();
                    if (!$payment->save()) {
                        if (!empty($member)) {
                            DB::connection($member->getConnectionName())->rollBack();
                        }
                        DB::connection($payment->getConnectionName())->rollBack();
                        continue;
                    }
                    DB::connection($payment->getConnectionName())->commit();
                    if (!empty($member)) {
                        DB::connection($member->getConnectionName())->commit();
                    }
                    $transInDb->push($payment);
                    Cache::put('momo_histories', $transInDb);
                    if (!$isTelegramAlert) {
                        \Telegram::sendMessage([
                            'chat_id' => env('TELEGRAM_CHAT_ID', '-1'), 
                            'text' => "\xE2\x9A\xA0 \[".date('Y-m-d H:i:s')."]_Undetected Member Charging_\n*Momo*\n*Số tiền:* ".$data['amount']."\n*Nội dung:* ".$data['comment']."\n*Người gửi:* ".$data['partnerName'],
                            'parse_mode' => "Markdown"
                        ]);
                    }
                }
            }
        }
    }

    private function getMomoHistories()
    {
        $endpoint = "https://api.web2m.com/historyapimomo/" . env('WEB2M_MOMO_TOKEN', '');
        $client = new \GuzzleHttp\Client(['verify' => false]);
        $response = $client->request('GET', $endpoint);
        $statusCode = $response->getStatusCode();
        $transList = [];
        if ($statusCode == 200) {
            $body = $response->getBody();
            $body = (string)$body;
            $content = json_decode($body, true);
            $transList = !empty($content['momoMsg']['tranList']) ? $content['momoMsg']['tranList'] : [];
        }
        return $transList;
    }
}

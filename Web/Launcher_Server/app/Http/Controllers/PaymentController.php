<?php

namespace App\Http\Controllers;

use App\Models\CoinLog;
use App\Http\Requests\CardCallbackRechargeRequest;
use App\Http\Requests\RechargeCardRequest;
use App\Models\LogCardCham;
use App\Models\Member;
use App\Models\MemberHistory;
use App\Models\Setting;
use App\PaymentMomo;
use App\Models\PaymentMomoHistory;
use Illuminate\Support\Facades\Cache;
use Illuminate\Support\Facades\DB;
use SimpleSoftwareIO\QrCode\Facades\QrCode;

class PaymentController extends \App\Http\BaseController
{
    public function rechargeCard(RechargeCardRequest $request)
    {
        $cardTypeNumber = $request->input('card_type');
        $cardType = "";
        switch ($cardTypeNumber) {
            case 'viettel':
                $cardType = "Viettel";
                break;
            case 'mobifone':
                $cardType = "Mobifone";
                break;
            case 'vinaphone':
                $cardType = "Vinaphone";
                break;
            case 'gate':
                return response()->json(['message' => 'Thẻ Gate hiện tại đã bị đóng lại do gặp lỗi, vui lòng liên hệ quản trị viên để được hỗ trợ!'], 400);
                $cardType = "Gate";
                break;
            case 'vietnamobile':
                $cardType = "Vietnamobile";
                break;
            case 'zing':
                return response()->json(['message' => 'Thẻ Zing hiện tại đã bị đóng lại do gặp lỗi, vui lòng liên hệ quản trị viên để được hỗ trợ!'], 400);
                $cardType = "Zing";
                break;
            case 'vcoin':
                return response()->json(['message' => 'Thẻ Vcoin hiện tại đã bị đóng lại do gặp lỗi, vui lòng liên hệ quản trị viên để được hỗ trợ!'], 400);
                $cardType = "Vcoin";
                break;
        }

        $serial = $request->input('serial');
        $pin = $request->input('pin');
        $cardAmount = $request->input('card_amount');
        $content = md5(uniqid('', true));
        $apiKey = env('THESIEUTOC_API');
        if (empty($apiKey)) {
            return response()->json(['message' => 'Hệ thống nạp thẻ chưa sẵn sàng, liên hệ quản trị viên để được hỗ trợ!'], 400);
//            $this->error('Hệ thống nạp thẻ chưa sẵn sàng, liên hệ quản trị viên để được hỗ trợ!');
        }
        $check = LogCardCham::where('Seri', $serial)
            ->where('Passcard', $pin)
            ->where('status', 1)
            ->get();
        if (!empty($check) && $check->count()) {
            return response()->json(['message' => 'Thẻ này đã được sử dụng!'], 400);
//            $this->error('Thẻ đã được sử dụng!');
        }

        $url = "https://thesieutoc.net/chargingws/v2?APIkey=" . $apiKey . "&mathe=" . $pin . "&seri=" . $serial . "&type=" . ucfirst($cardType) . "&menhgia=" . $cardAmount . "&content=" . $content;
        $ch = curl_init();
        curl_setopt($ch, CURLOPT_URL, $url);
        curl_setopt($ch, CURLOPT_CAINFO, base_path() . '/curl-ca-bundle.crt');
        curl_setopt($ch, CURLOPT_CAPATH, base_path() . '/curl-ca-bundle.crt');
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        $out = json_decode(curl_exec($ch));
        $httpCode = 0;
        if (isset($out->status)) {
            $httpCode = 200;
        }
        curl_close($ch);
        if ($httpCode == 200) {
            if ($out->status == '00' || $out->status == 'thanhcong') {
                //Gửi thẻ thành công, đợi duyệt.
                $member = auth()->user();
                $logCardCham = new LogCardCham();
                $logCardCham->UserName = $member->Email;
                $logCardCham->NameCard = ucfirst($cardType);
                $logCardCham->Money = $cardAmount;
                $logCardCham->Seri = $serial;
                $logCardCham->Passcard = $pin;
                $logCardCham->Timer = date('Y-m-d H:i:s');
                $logCardCham->Active = 1;
                $logCardCham->Success = 0;
                $logCardCham->TaskID = $content;
                $logCardCham->status = 0;

                if ($logCardCham->save()) {
//                    $this->success($out->msg);
                    return response()->json(['message' => $out->msg], 200);
                }
                return response()->json(['message' => 'Gửi thẻ thành công, tuy nhiên hệ thống bị lỗi, hãy liên hệ quản trị viên để được trợ giúp!'], 400);
            } else if ($out->status != '00' && $out->status != 'thanhcong') {
                // thất bại ở đây
                return response()->json(['message' => $out->msg], 400);
            }
        } else {
            return response()->json(['message' => 'Hệ thống nạp thẻ bị lỗi, liên hệ quản trị viên để được hỗ trợ!'], 400);
        }
    }

    public function cardCallback(CardCallbackRechargeRequest $request)
    {
        $status = $request->input('status');
        $serial = $request->input('serial');
        $pin = $request->input('pin');
        $cardType = $request->input('card_type');
        $amount = $request->input('amount');
        $realAmount = $request->input('real_amount');
        $content = $request->input('content');
        $cardOnhold = LogCardCham::where('Seri', $serial)
            ->where('Passcard', $pin)
            ->where('TaskID', $content)
            ->where('Active', 1)
            ->where('status', 0)
            ->first();
        if ($cardOnhold) {
            if ($status == 'thanhcong') {
                //Xử lý nạp thẻ thành công tại đây.
                $cardOnhold->Active = 0;
                $cardOnhold->status = 1;
                if ($cardOnhold->save()) {
                    $member = Member::where('Email', $cardOnhold->UserName)
                        ->first();
                    if ($member) {
                        if ($member->getTotalCharged() > 0) {
                            $member->Money += floor($amount * Setting::get('he-so-atm') * (100 + intval(Setting::get('khuyen-mai-nap-coin'))) / 100);
                        } else {
                            $member->Money += floor($amount * Setting::get('he-so-atm') * (100 + intval(Setting::get('khuyen-mai-nap-lan-dau'))) / 100);
                        }
                        $member->save();
                        $memHistory = new MemberHistory();
                        $memHistory->memberReChargeMoneyLog($member->UserID, $amount, $this->historyCardType($cardType), '');
                    }
                }
            } else if ($status == 'saimenhgia') {
                //Xử lý nạp thẻ sai mệnh giá tại đây.
                $cardOnhold->status = 3; //sai menh gia
                $cardOnhold->Active = 0; //sai menh gia
                $cardOnhold->save();
            } else {
                //Xử lý nạp thẻ thất bại tại đây.
                $cardOnhold->status = 2; //nap that bai
                $cardOnhold->Active = 0; //sai menh gia
                $cardOnhold->save();
            }
        }
    }

    public function historyCardType($type)
    {
        switch ($type) {
            case 'Viettel':
                return 11;
            case 'Mobifone':
                return 13;
            case 'Vinaphone':
                return 12;
            case 'Gate':
                return 14;
            case 'Vietnamobile':
                return 15;
            case 'Zing':
                return 16;
            case 'Vcoin':
                return 17;
        }
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
                if ($status == 999) {
                    $comment = $transaction['comment'];
                    if (!empty($comment)) {
                        $member = Member::where('Email', strtolower($comment))->first();
                        if (!empty($member)) {
                            $amount = $transaction['amount'];
                            $member->Money += floor($amount * Setting::get('he-so-atm'));
                            DB::connection($member->getConnectionName())->beginTransaction();
                            if (!$member->save()) {
                                DB::connection($member->getConnectionName())->rollBack();
                                continue;
                            }
                        }
                    }
                    $data = [
                        'comment' => $transaction['comment'],
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
                }
            }
        }
    }

    private function getMomoHistories()
    {
        $endpoint = "https://api.web2m.com/historyapimomo/" . env('WEB2M_MOMO_TOKEN', '');
        $client = new \GuzzleHttp\Client();
        $response = $client->request('GET', $endpoint);
        $statusCode = $response->getStatusCode();
        $transList = [];
        if ($statusCode == 200) {
            $content = json_decode($response->getBody(), true);
            $transList = !empty($content['momoMsg']['tranList']) ? $content['momoMsg']['tranList'] : [];
        }
        return $transList;
    }

    public function getBankQrCode()
    {
        $member = auth()->user();
        $bankAccountId = env('WEB2M_ACB_ACCOUNT_NUMBER', '');
        $client_id = env('VIETQR_CLIENT_ID', '');
        $api_key = env('VIETQR_API_KEY', '');
        if (empty($bankAccountId) || empty($client_id) || empty($api_key)) {
            return $this->error('Hệ thống gặp lỗi, vui lòng liên hệ quản trị viên để được trợ giúp!');
        }
        $content = env('CHARGE_PREFIX', 'EZG').$member->UserID;
        $res = $this->vietQrCurl($content);
        $res = json_decode($res);
        if ($res->code == "00") {
            $accinfo = [
                'name' => 'PHAM TRUNG KIEN',
                'acc_num' => $bankAccountId,
                'bank_name' => 'Á CHÂU (ACB)'
            ];
            return $this->success('OK', ['src' => $res->data->qrDataURL, 'comment' => $content, 'accinfo' => $accinfo]);
        }
        return $this->error('Hệ thống gặp lỗi, vui lòng liên hệ quản trị viên để được trợ giúp!');
    }

    private function vietQrCurl($content)
    {
        $curl = curl_init();

        curl_setopt_array($curl, array(
            CURLOPT_URL => 'https://api.vietqr.io/v2/generate',
            CURLOPT_RETURNTRANSFER => true,
            CURLOPT_ENCODING => '',
            CURLOPT_MAXREDIRS => 10,
            CURLOPT_TIMEOUT => 0,
            CURLOPT_FOLLOWLOCATION => true,
            CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
            CURLOPT_CUSTOMREQUEST => 'POST',
            CURLOPT_POSTFIELDS => '{
                "accountNo": "'.env('WEB2M_ACB_ACCOUNT_NUMBER', '').'",
                "accountName": "PHAM TRUNG KIEN",
                "acqId": "970416",
                "addInfo": "'.$content.'",
                "template": "compact"
            }',
            CURLOPT_HTTPHEADER => array(
                'x-client-id: '.env('VIETQR_CLIENT_ID', ''),
                'x-api-key: '.env('VIETQR_API_KEY', ''),
                'Content-Type: application/json',
            ),
        ));

        $response = curl_exec($curl);

        curl_close($curl);
        return $response;
    }

    public function getMomoChargeQr()
    {
        $member = auth()->user();
        $momoQrText = '2|99|0868862312|TRAN THI NGUYET||0|0|0|'.env('CHARGE_PREFIX', '').$member->UserID.'|transfer_myqr';
        $momoQr = QrCode::format('png')->size(250)->generate($momoQrText);
        $accinfo = [
            'name' => 'TRAN THI NGUYET',
            'acc_num' => '0868862312',
            'bank_name' => 'MOMO'
        ];
        return $this->success('', ['src' => 'data:image/png;base64,'.base64_encode($momoQr), 'comment' => env('CHARGE_PREFIX', 'EZG').$member->UserID, 'accinfo' => $accinfo]);
    }
}

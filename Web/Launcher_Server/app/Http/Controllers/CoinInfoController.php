<?php

namespace App\Http\Controllers;

use App\Models\Setting;

class CoinInfoController extends \App\Http\BaseController
{
    /**
     * @var \App\Models\Member $member
     */
    protected $member;
    public function index()
    {
        $member = auth()->user();
        $this->member = $member;
        if($member->IsBan == 1) {
            return $this->error('Tài khoản của bạn đã bị khoá');
        }

        return $this->success('OK!', [
            'coin' => $member->Money
        ]);
    }

    public function chargerateinfo()
    {
        $data = [
            'rateCard' => number_format(10000 * Setting::get('he-so-nap-the'), 0, ',', '.'),
            'rateMomo' => number_format(10000 * Setting::get('he-so-atm'), 0, ',', '.'),
            'rateATM' => number_format(10000 * Setting::get('he-so-atm'), 0, ',', '.'),
        ];
        return $this->success('OK!', $data);
    }
}

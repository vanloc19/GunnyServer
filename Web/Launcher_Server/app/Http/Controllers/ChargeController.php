<?php

namespace App\Http\Controllers;

use App\Helpers\Request;
use App\Http\Requests\MemberLoginRequest;
use App\Models\Member;
use App\Models\MemberHistory;
use App\Models\MemberToken;
use App\Models\Player;
use App\Models\ServerList;
use App\Models\Setting;
use Illuminate\Support\Facades\DB;

class ChargeController extends \App\Http\BaseController
{
    public function index($player, $coin, $server_id = 1001)
    {
        $member = Member::where('Email', $player)->first();
        $server = ServerList::find($server_id);
        $player = Player::select('UserID')->where('UserName', $player)->first();
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
            return $this->success('Chuyển xu thành công, nhận xu trong game nhé!', ['coin' => $member->Money]);

        } else {
            return $this->error('Không thể nạp xu vào game, vui lòng thoát game và thử lại hoặc thông báo tới quản trị viên để được trợ giúp!');
        }
    }
}

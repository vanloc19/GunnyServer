<?php

namespace App\Http\Controllers;

use App\Models\Player;

class PlayerListController extends \App\Http\BaseController
{
    public function index()
    {
        $member = auth()->user();
        $columns = [
            'UserID',
            'NickName'
        ];
        $players = Player::select($columns)->where('UserName', $member->Email)->get()->toArray();
        return $this->success('Thành công', $players);
    }
}

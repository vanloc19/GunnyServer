<?php

namespace App\Http\Controllers;

use App\Models\MemberHistory;
use App\Models\Player;

class ChargeHistoryController extends \App\Http\BaseController
{
    public function index()
    {
        $member = auth()->user();
        $histories = MemberHistory::where('UserID', $member->UserID)->orderBy('TimeCreate', 'desc')->get();
        if (!empty($histories) && $histories->count() > 0) {
            foreach ($histories as $history) {
                $history->TimeCreate = date('d-m-Y H:i:s', $history->TimeCreate);
            }
        }
        return $this->success('Thành công', $histories);
    }
}

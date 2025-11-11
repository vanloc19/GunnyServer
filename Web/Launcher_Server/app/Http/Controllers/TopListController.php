<?php

namespace App\Http\Controllers;

use App\Models\CoinLog;
use App\Models\MemberHistory;
use App\Models\MissionCount;
use App\Models\Player;
use App\Models\UserLogs;
use Illuminate\Support\Facades\DB;

class TopListController extends \App\Http\BaseController
{
    public function index()
    {
        $toplc = DB::connection('sqlsrv_tank41')->table('Sys_Users_Detail')->select('Sys_Users_Detail.NickName', 'Sys_Users_FightPower.FightPower')
            ->where('Sys_Users_Detail.UserID', '!=', 980)
            ->where('Sys_Users_Detail.UserID', '!=', 5001)
            ->join('Sys_Users_FightPower', 'Sys_Users_Detail.UserID', '=', 'Sys_Users_FightPower.UserID')
            ->orderBy('Sys_Users_FightPower.FightPower', 'DESC')
            ->take(20)->get()->toArray();
//        $toplv = Player::select('NickName', 'Grade')->orderBy('Grade', 'DESC')->take(10)->get()->toArray();
//        $topwin = Player::select('NickName', 'Win')->orderBy('Win', 'DESC')->take(10)->get()->toArray();
//        $toponline = Player::select('NickName', 'OnlineTime')->orderBy('OnlineTime', 'DESC')->take(10)->get()->toArray();
        $toplctt = DB::connection('sqlsrv_tank41')->table('Sys_Users_Detail')->select('Sys_Users_Detail.NickName', 'Sys_Users_FightPower.FightPower')
            ->where('Sys_Users_Detail.Date', '>=', '2023-04-29 00:00:00')
            ->where('Sys_Users_Detail.UserID', '!=', 980)
            ->where('Sys_Users_Detail.UserID', '!=', 5001)
            ->join('Sys_Users_FightPower', 'Sys_Users_Detail.UserID', '=', 'Sys_Users_FightPower.UserID')
            ->orderBy('Sys_Users_FightPower.FightPower', 'DESC')
            ->take(20)->get()->toArray();
//        $toplvtt = Player::select('NickName', 'Grade')->where('Date', '>=', '2023-01-17 00:00:00')->orderBy('Grade', 'DESC')->take(10)->get()->toArray();
//        $memhis = MemberHistory::where('TypeCode', '!=', 3)
//            ->where('TimeCreate', '>=', strtotime('2023-04-15 00:00:00'))
//            ->where('TimeCreate', '<=', strtotime('2023-04-28 23:59:59'))
//            ->get();
//        $coinlog = CoinLog::where('Time', '>=', '2023-04-15 00:00:00')
//            ->where('Time', '<=', '2023-04-28 23:59:59')
//            ->where('Value', '>', 0)->get();
//        $topnap = [];
//        foreach ($memhis as $his) {
//            if (!empty($topnap[$his->UserID])) {
//                $topnap[$his->UserID]['Amount'] += $his->Value;
//                continue;
//            }
//            $player = Player::select('NickName')->where('UserName', $his->Member->Email)->first();
//            $item = [
//                'NickName' => $player->NickName,
//                'Amount' => $his->Value
//            ];
//            $topnap[$his->UserID] = $item;
//        }
//        foreach ($coinlog as $log) {
//            if (!empty($topnap[$log->MemberID])) {
//                $topnap[$log->MemberID]['Amount'] += $log->Value;
//                continue;
//            }
//            $player = Player::select('NickName')->where('UserName', $log->Member->Email)->first();
//            $item = [
//                'NickName' => $player->NickName,
//                'Amount' => $log->Value
//            ];
//            $topnap[$log->MemberID] = $item;
//        }
//        $topnap = array_filter($topnap, function ($a) {
//            return $a['NickName'] != 'Media Admin' && $a['NickName'] != 'Linda Tiktok' && $a['NickName'] != 'Phuong TIkTok';
//        });
//        uasort($topnap, function ($a, $b) {
//            return $a['Amount'] > $b['Amount'] ? -1 : 1;
//        });
//        $topnap = array_slice($topnap, 0, 10);
//        $topnap = array_values($topnap);
        $missionCounts = MissionCount::where('MissionID', 1375)->get();
        $toppb = [];
        foreach ($missionCounts as $missionCount) {
            if (!empty($toppb[$missionCount->UserID])) {
                $toppb[$missionCount->UserID]['Count'] += $missionCount->Count;
                continue;
            }
            $player = Player::select('NickName')->where('UserID', $missionCount->UserID)->first();
            $item = [
                'NickName' => $player->NickName,
                'Count' => $missionCount->Count
            ];
            $toppb[$missionCount->UserID] = $item;
        }
        uasort($toppb, function ($a, $b) {
            return $a['Count'] > $b['Count'] ? -1 : 1;
        });
        $toppb = array_slice($toppb, 0, 20);
        $toppb = array_values($toppb);

        $missionCounts2 = MissionCount::where('MissionID', 13204)->get();
        $toppb2 = [];
        foreach ($missionCounts2 as $missionCount) {
            if (!empty($toppb2[$missionCount->UserID])) {
                $toppb2[$missionCount->UserID]['Count'] += $missionCount->Count;
                continue;
            }
            $player = Player::select('NickName')->where('UserID', $missionCount->UserID)->first();
            $item = [
                'NickName' => $player->NickName,
                'Count' => $missionCount->Count
            ];
            $toppb2[$missionCount->UserID] = $item;
        }
        uasort($toppb2, function ($a, $b) {
            return $a['Count'] > $b['Count'] ? -1 : 1;
        });
        $toppb2 = array_slice($toppb2, 0, 20);
        $toppb2 = array_values($toppb2);

//        $tophd = Player::select('NickName', 'charmGP')
//            ->where('UserID', '!=', 980)
//            ->where('UserID', '!=', 5001)
//            ->orderBy('charmGP', 'DESC')
//            ->take(10)->get()->toArray();
        $historiesBuyItem = UserLogs::where('Type', 'Buy item')
            ->where('TimeCreate', '>=', '2023-04-30 00:00:00.000')
            ->get();
        $topTieuXu = [];
        foreach ($historiesBuyItem as $item) {
            if (empty($topTieuXu[$item->UserID])) {
                $topTieuXu[$item->UserID] = [
                    'UserID' => $item->UserID,
                    'Amount' => 0
                ];
            }
            $topTieuXu[$item->UserID]['Amount'] += intval($item->Content);
        }
        uasort($topTieuXu, function ($a, $b) {
            return $a['Amount'] > $b['Amount'] ? -1 : 1;
        });
        $topTieuXu = array_slice($topTieuXu, 0, 10);
        $topTieuXu2 = [];
        foreach ($topTieuXu as $top) {
            if (!empty($topTieuXu2[$top['UserID']])) {
                $topTieuXu2[$top['UserID']]['Amount'] += $top['Amount'];
                continue;
            }
            $player = Player::select('NickName')->where('UserID', $top['UserID'])->first();
            $item = [
                'NickName' => $player->NickName,
                'Amount' => $top['Amount']
            ];
            $topTieuXu2[$top['UserID']] = $item;
        }
        $topTieuXu2 = array_values($topTieuXu2);

        return $this->success('Thành công', [
            'lc' => $toplc,
//            'lv' => $toplv,
            'lctt' => $toplctt,
//            'lvtt' => $toplvtt,
//            'win' => $topwin,
//            'online' => $toponline,
//            'nap' => $topnap,
            'pbgah' => $toppb,
            'pbdtds' => $toppb2,
//            'hd' => $tophd,
            'tieuxu' => $topTieuXu2,
        ]);
    }
}

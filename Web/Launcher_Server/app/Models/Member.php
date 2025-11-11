<?php


namespace App\Models;

use App\Helpers\Request;
use Illuminate\Support\Str;
use Illuminate\Foundation\Auth\User as Authenticatable;

class Member extends Authenticatable
{
    protected $connection = 'sqlsrv_mem';

    protected $table = 'Mem_Account';

    protected $fillable = ['Email', 'Password', 'Fullname', 'Phone', 'Money', 'MoneyLock', 'TotalMoney', 'MoneyEvent', 'Point', 'CountLucky', 'VIPLevel', 'VIPExp', 'IsBan', 'IPCreate', 'AllowSocialLogin', 'TimeCreate', 'Pass2', 'ActiveIP'];

    protected $hidden = [
        'Password', 'Pass2',
    ];

    protected $primaryKey = 'UserID';

    public $timestamps = false;

    public function chargeMoney($coin)
    {
        $this->Money -= $coin;
        return $this->save();
    }

    public function BanReason()
    {
//        if (!$this->IsBan) {
//            return '';
//        }
//        $reason = BanMemberReason::where('UserID', $this->UserID)->first();
//        if (!empty($reason)) {
//            return $reason->reason;
//        }
//        return '';
    }

    private function getToken()
    {
        $token = Request::getTokenAuthorization();
        if ($token && !(preg_match('/Bearer\s(\S+)/', $token))) {
            return false;
        }
        $token = str_replace('Bearer', '', $token);
        return trim($token);
    }

    public function createToken()
    {
        return Str::random(32);
    }

    public function getMemberId()
    {
        $token = $this->getToken();
    }

    public function isLoggedIn()
    {
        //
    }

    public function current()
    {
        //
    }

    public function Histories()
    {
        return $this->hasMany(MemberHistory::class, 'UserID', 'UserID');
    }

    public function CoinLogs()
    {
        return $this->hasMany(CoinLog::class, 'MemberID', 'UserID');
    }

    public function getTotalCharged()
    {
        $memhis = $this->Histories;
        $coinlog = $this->CoinLogs;
        $amount = 0;
        foreach ($memhis as $his) {
            if ($his->TypeCode == 3) {
                continue;
            }
            $amount += $his->Value;
        }
        foreach ($coinlog as $log) {
            if ($log->Value <= 0) {
                continue;
            }
            $amount += $log->Value;
        }
        return $amount;
    }
}

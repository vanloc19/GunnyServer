<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class MemberHistory extends Model
{
    protected $connection = 'sqlsrv_mem';

    protected $table = 'Mem_History';
    protected $primaryKey = 'ID';
    protected $fillable = ['UserID'];
    protected $guarded = ['ID'];
    public $timestamps = false;
    // protected $hidden = [];
    // protected $dates = [];

    /*
    |--------------------------------------------------------------------------
    | FUNCTIONS
    |--------------------------------------------------------------------------
    */
    public static function historyTypeCodeMeaning($type = null)
    {
        $arr = [
            0 => 'Không xác định',
            1 => 'Nạp coin qua Momo',
            2 => 'Nạp coin qua ACB',
            3 => 'Chuyển xu vào game',
            6 => 'Đổi tên nhân vật',
            11 => 'Nạp thẻ Viettel',
            12 => 'Nạp thẻ Vinaphone',
            13 => 'Nạp thẻ Mobifone',
            14 => 'Nạp thẻ Gate',
            15 => 'Nạp thẻ Vietnamobile',
            16 => 'Nạp thẻ Zing',
            17 => 'Nạp thẻ Vcoin',
        ];
        if (!empty($type)) {
            return $arr[$type];
        }
        return $arr;
    }
    public function memberReChargeMoneyLog($UserID, $Value, $type, $ip)
    {

        $this->UserID = $UserID;
        $this->Type = 'Nạp coin';
        $this->TypeCode = $type;
        $this->Content = self::historyTypeCodeMeaning($type);
        $this->Value = $Value;
        $this->TimeCreate = time();
        $this->IPCreate = $ip;
        return $this->save();
    }

    public function memberChargeMoneyLog($UserID, $Value, $ServerName, $ip)
    {
        $this->UserID = $UserID;
        $this->Type = 'Chuyển xu';
        $this->TypeCode = 3;
        $this->Content = 'Chuyển '.$Value*Setting::get('he-so-doi-coin').' vào máy chủ '.$ServerName;
        $this->Value = $Value;
        $this->TimeCreate = time();
        $this->IPCreate = $ip;
        return $this->save();
    }

    public function memberTotalChargedToday($UserID)
    {
        $total_changed = 0;
        $rows = self::where('UserID', $UserID)
            ->where('TypeCode', 3)
            ->where('TimeCreate', '>=', strtotime(date('Y-m-d 00:00:00')))
            ->get();
        if (!empty($rows) && $rows->count()) {
            foreach ($rows as $history) {
                $total_changed += abs(intval($history->Value));
            }
        }
        return $total_changed;
    }

    public function memberChangeNickNameLog($UserID, $previousNickName, $newNickName, $Value, $ServerName, $ip)
    {
        $this->UserID = $UserID;
        $this->Type = 'Đổi tên';
        $this->TypeCode = 6;
        $this->Content = 'Đổi tên nhân vật từ '.$previousNickName.' thành '.$newNickName.' trong máy chủ '.$ServerName;
        $this->Value = $Value;
        $this->TimeCreate = time();
        $this->IPCreate = $ip;
        return $this->save();
    }

    public function Member()
    {
        return $this->hasOne('App\Models\Member','UserID','UserID');
    }
}

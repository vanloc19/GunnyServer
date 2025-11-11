<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class LogCardCham extends Model
{
    protected $connection = 'sqlsrv_mem';

    protected $table = 'WSs_LogCardCham';
    protected $primaryKey = 'TaskID';
    public $timestamps = false;
    protected $guarded = false;
    protected $fillable = ['UserName', 'NameCard', 'Money', 'Seri', 'Passcard', 'Timer', 'Active', 'Success', 'TaskID', 'status'];
    // protected $hidden = [];
    // protected $dates = [];
    public $incrementing = false;

    //FUNCTION FROM GOGUN
    public function getStatusAttribute($value)
    {
        //$value = status
        if ($value == 0 && $this->Active == 1)
            return '<span style="color: blue">Đang xử lý</span>';
        if ($value == 1 && $this->Active == 0)
            return '<span style="color: green">Thành công</span>';
        if ($value == 2 && $this->Active == 0)
            return '<span style="color: red">Thất bại</span>';
        if ($value == 3 && $this->Active == 0)
            return '<span style="color: orange">Sai mệnh giá</span>';
    }
}

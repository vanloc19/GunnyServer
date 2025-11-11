<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class PaymentMomo extends Model
{
    protected $connection = 'sqlsrv_mem';

    protected $table = 'PaymentMoMo';
    protected $primaryKey = 'id';
    protected $fillable = ['id', 'UserID', 'TransID', 'Amount', 'created_at', 'updated_at'];

    public $timestamps = true;
    
    public function findTransaction($transID)
    {
        $rows = self::where('TransID', $transID)->get();
        return $rows->count();
    }
}

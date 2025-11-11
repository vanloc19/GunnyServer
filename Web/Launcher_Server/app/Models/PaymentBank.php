<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class PaymentBank extends Model
{
    protected $connection = 'sqlsrv_mem';

    protected $table = 'Payment_Bank';
    protected $primaryKey = 'id';
    protected $fillable = ['id', 'UserID', 'TransID', 'Amount', 'Vendor', 'created_at', 'updated_at'];
    public $timestamps = true;

    
    public function findTransaction($transID)
    {
        $rows = self::where('TransID', $transID)->get();
        return $rows->count();
    }
}

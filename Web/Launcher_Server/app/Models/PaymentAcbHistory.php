<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class PaymentAcbHistory extends Model
{
    protected $table = 'payment_acb_history';
    public $incrementing = true;
    public $timestamps = true;

    protected $fillable = ['id', 'transactionID', 'amount', 'description'];
}

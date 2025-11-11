<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class CoinLog extends Model
{

    /*
    |--------------------------------------------------------------------------
    | GLOBAL VARIABLES
    |--------------------------------------------------------------------------
    */
    protected $connection = 'sqlsrv';
    protected $table = 'coin_logs';
    // protected $primaryKey = 'id';
    // public $timestamps = false;
    protected $guarded = ['id'];
    // protected $fillable = [];
    // protected $hidden = [];
    // protected $dates = [];
    public $timestamps = false;

    /*
    |--------------------------------------------------------------------------
    | FUNCTIONS
    |--------------------------------------------------------------------------
    */

    /*
    |--------------------------------------------------------------------------
    | RELATIONS
    |--------------------------------------------------------------------------
    */
    public function User()
    {
        return $this->hasOne('App\User', 'id', 'UserID');
    }

    public function Member()
    {
        return $this->hasOne('App\Models\Member', 'UserID', 'MemberID');
    }

    public function CoinLogMember()
    {
        return $this->hasOne('App\Models\CoinLogMember', 'UserID', 'MemberID');
    }

    /*
    |--------------------------------------------------------------------------
    | SCOPES
    |--------------------------------------------------------------------------
    */

    /*
    |--------------------------------------------------------------------------
    | ACCESSORS
    |--------------------------------------------------------------------------
    */

    /*
    |--------------------------------------------------------------------------
    | MUTATORS
    |--------------------------------------------------------------------------
    */

    public function setTimeAttribute($value)
    {
        $this->attributes['Time'] = date('Y-m-d H:i:s');
    }
}

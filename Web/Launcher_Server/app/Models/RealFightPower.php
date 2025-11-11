<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class RealFightPower extends Model
{

    protected $connection = 'sqlsrv_tank41';

    protected $table = 'Sys_Users_FightPower';

    protected $fillable = ['UserID', 'FightPower'];

    protected $primaryKey = 'ServerID';

    public $timestamps = false;
}

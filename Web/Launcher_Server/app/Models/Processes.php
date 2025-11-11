<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class Processes extends Model
{

//    protected $connection = 'sqlsrv_tank41';

    protected $table = 'User_Processes';

    protected $fillable = ['UserName', 'Processes'];

    protected $primaryKey = 'ID';

    public $timestamps = true;
}

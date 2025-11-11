<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class ServerList extends Model
{

    protected $connection = 'sqlsrv_mem';

    protected $table = 'Server_List';

    protected $fillable = ['ServerName', 'Host', 'Username', 'Password', 'Database', 'Connection', 'LinkCenter', 'LinkRequest', 'LinkConfig', 'LinkConfigRegister'];

    protected $primaryKey = 'ServerID';


    public function currentStatus()
    {
        switch ($this->Status){
            case 0:
                return "Đang bảo trì";
            case 1:
                return "Hoạt động";
            case 2:
                return "Sắp ra mắt";
            default:
                return "Không xác định";
        }
    }

    public $timestamps = false;
}

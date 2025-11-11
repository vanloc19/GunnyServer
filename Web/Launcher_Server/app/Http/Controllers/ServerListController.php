<?php

namespace App\Http\Controllers;

use App\Models\ServerList;

class ServerListController extends \App\Http\BaseController
{
    public function index()
    {
        $columns = [
            'ServerID',
            'ServerName'
        ];
        $servers = ServerList::select($columns)->get()->toArray();
        return $this->success('Thành công', $servers);
    }
}

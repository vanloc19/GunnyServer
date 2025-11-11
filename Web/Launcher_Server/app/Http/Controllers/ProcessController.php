<?php

namespace App\Http\Controllers;

use App\Http\Requests\ProcessRequest;
use App\Models\Processes;

class ProcessController extends \App\Http\BaseController
{
    public function index(ProcessRequest $request)
    {
        $username = $request->username;
        $processes = $request->process;
        if (strpos($processes, 'Tool')) {
            $row = new Processes();
            $row->UserName = $username;
            $row->Processes = $processes;
            $row->save();
        }
    }
}

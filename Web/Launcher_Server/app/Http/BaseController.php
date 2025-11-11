<?php

namespace App\Http;

class BaseController extends \Illuminate\Routing\Controller
{

    protected function success($message = '', $data = [], $status = true)
    {
        $returnval = ['success' => $status, 'message' => $message];
        if (!empty($data)) {
            $returnval['data'] = $data;
        }
        return response()->json($returnval);
    }

    protected function error($message = '', $data = [], $status = false)
    {
        $returnval = ['success' => $status, 'message' => $message];
        if (!empty($data)) {
            $returnval['data'] = $data;
        }
        return response()->json($returnval);
    }

}

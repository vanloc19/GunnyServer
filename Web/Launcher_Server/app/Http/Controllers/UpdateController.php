<?php

namespace App\Http\Controllers;

class UpdateController extends \App\Http\BaseController
{
    public function index($platform, $appversion)
    {
        $current_app = env('LAUNCHER_APP_VERSION', '1.0.0');
        if ($current_app != $appversion) {
            return response($current_app);
        }
        return response('0');
    }

    public function getDownload($platform = null)
    {
        $current_app = env('LAUNCHER_APP_VERSION', '1.0.0');
        if ($platform == 'win32') {
            $file_name = 'EasyGunnyLauncherFull.exe';
        } elseif ($platform == 'darwin') {
            $file_name = 'EasyGunnyLauncherFull.zip';
        } else {
            $platform = 'win32';
            $file_name = 'EasyGunnyLauncherFull.exe';
        }
        $path = public_path('updates' . DIRECTORY_SEPARATOR . $current_app . DIRECTORY_SEPARATOR . $platform . DIRECTORY_SEPARATOR . $file_name);

        $headers = array(
            'Content-Type: application/octet-stream',
        );

        return response()->download($path, $file_name, $headers);
    }
}

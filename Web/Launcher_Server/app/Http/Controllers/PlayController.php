<?php

namespace App\Http\Controllers;

use App\Helpers\Request;
use App\Http\Requests\GetPlayVarsRequest;
use App\Http\Requests\MemberLoginRequest;
use App\Models\Member;
use App\Models\MemberToken;
use App\Models\Player;
use App\Models\ServerList;

class PlayController extends \App\Http\BaseController
{
    public function index()
    {
        /** @var Member $user */
        $user = auth()->user();
        if ($user->IsBan) {
            $this->error('Tài khoản của bạn đã bị khóa!');
        }
//        if (!empty($user['2fa']) && !empty($user->Fullname)) {
//            $token = $request->token;
//            $launcherTokens = LauncherToken::where('MemberID', $user->UserID)
//                ->where('token', $token)
//                ->where('status', 0)
//                ->get();
//            if (!empty($launcherTokens) && $launcherTokens->count()) {
//                $last_token = $launcherTokens->first();
//                $last_time = strtotime($last_token->created_at);
//                $time = strtotime('now') - $last_time;
//                if ($time > env('2FA_LAUNCHER_TOKEN_EXPIRES', 7200)) {
//                    $this->error('Phiên đăng nhập hết hạn!', [], self::TOKEN_EXPIRED);
//                }
//                foreach ($launcherTokens as $item) {
//                    $item->status = 2;
//                    $item->save();
//                }
//            } else {
//                $this->error('Yêu cầu không hợp lệ!');
//            }
//        }
        $svid = 1001;
        $bypass = false;
        $serverInfo = ServerList::find($svid);
        if (empty($serverInfo)) {
            return $this->error('Server Die!');
        }
        $keyrand = rand(111111, 999999);
        $timeNow = time();
        $requestLink = $serverInfo->LinkRequest;
        $content = file_get_contents( trim($requestLink, '/'). "/CreateLogin.aspx?content=" . $user->Email . "|" . strtoupper($keyrand) . "|" . $timeNow . "|" . md5($user->Email . strtoupper($keyrand) . $timeNow . env('KEY_REQUEST', 'no-one_is_promised_tomorow')));
        if (trim($content) != "0") {
            return $this->error($content);
        }
//        $player = Player::select('UserID', 'LoginDevice')->where('UserName', $username)->first();
//        if (!empty($player)) {
//            $player->LauncherLoggedin();
//        }
//        $text = trim(env('FLASH_URL', 'https://go-gun.com'), '/').'Loading.swf?user=' . $user->Email . '&key=' . $keyrand . '&v=10950&rand=' . rand(100000000, 999999999) . '&config=' . $serverInfo->LinkConfig . '&sessionId=' . rand(100000000, 999999999);//dang nhap thanh cong
        return $this->success('', [
            'user' => $user->Email,
            'keyrand' => $keyrand,
            'configLink' => $serverInfo->LinkConfig,
        ]);
    }

    public function play($token)
    {
        $UserId = MemberToken::checkToken($token);
        $email = '';
        $keyrand = '';
        $configLink = '';
        $flashFile = 'Loading.swf';
        $flashUrl = '';
        if (empty($UserId)) {
            $error = "Phiên đăng nhập hết hạn!";
        } else {
            /** @var Member $user */
            $user = Member::find($UserId);
            if (empty($user)) {
                $error = "Thông tin đăng nhập không đúng!";

            } else {
                $error = '';
                if ($user->IsBan) {
                    $error = 'Tài khoản của bạn đã bị khóa!';
                }
                $svid = 1001;
                $bypass = false;
                $serverInfo = ServerList::find($svid);
                if (empty($serverInfo)) {
                    $error ='Server Die!';
                }
                $keyrand = rand(111111, 999999);
                $timeNow = time();
                $requestLink = $serverInfo->LinkRequest;
                $content = file_get_contents(trim($requestLink, '/') . "/CreateLogin.aspx?content=" . $user->Email . "|" . strtoupper($keyrand) . "|" . $timeNow . "|" . md5($user->Email . strtoupper($keyrand) . $timeNow . env('KEY_REQUEST', 'no-one_is_promised_tomorow')));
                if (trim($content) != "0") {
                    $error = $content;
                }
                if (empty($error)) {
					$configLink = $serverInfo->LinkConfig;
					$flashUrl = trim($serverInfo->LinkFlash, '/');
                    $email = $user->Email;
                }
            }
        }
        return view('playLauncher', [
            'error' => $error,
            'username' => $email,
            'keyrand' => $keyrand,
            'configLink' => $configLink,
            'flashFile' => $flashFile,
            'flashUrl' => $flashUrl
        ]);
    }

    public function getPlayVars(GetPlayVarsRequest $request)
    {
        $member = auth()->user();
        $UserId = $member->UserID;
        $email = '';
        $keyrand = '';
        $configLink = '';
        $flashFile = 'Loading.swf';
        $flashUrl = '';
        if (empty($UserId)) {
            $error = "Phiên đăng nhập hết hạn!";
        } else {
            /** @var Member $user */
            $user = Member::find($UserId);
            if (empty($user)) {
                $error = "Thông tin đăng nhập không đúng!";

            } else {
                $error = '';
                if ($user->IsBan) {
                    $error = 'Tài khoản của bạn đã bị khóa!';
                }
                $svid = 1;
                $bypass = false;
                $serverInfo = ServerList::find($svid);
                if (empty($serverInfo)) {
                    $error ='Server Die!';
                }
                $keyrand = rand(111111, 999999);
                $timeNow = time();
                $requestLink = $serverInfo->LinkRequest;
                $content = file_get_contents(trim($requestLink, '/') . "/CreateLogin.aspx?content=" . $user->Email . "|" . strtoupper($keyrand) . "|" . $timeNow . "|" . md5($user->Email . strtoupper($keyrand) . $timeNow . env('KEY_REQUEST', 'no-one_is_promised_tomorow')));
                if (trim($content) != "0") {
                    $error = $content;
                }
                if (empty($error)) {
                    $player = Player::select('UserID', 'LoginDevice')->where('UserName', $user->Email)->first();
                    if (!empty($player)) {
                        $configLink = $serverInfo->LinkConfig;
                        $flashUrl = trim(env('FLASH_URL'), '/');
                    } else {
                        $configLink = $serverInfo->LinkConfigRegister;
                        $flashUrl = trim(env('FLASH_URL_REGISTER'), '/');
                    }
                    $email = $user->Email;
                }
            }
        }
        return response()->json([
            'error' => $error,
            'username' => $email,
            'keyrand' => $keyrand,
            'configLink' => $configLink,
            'flashFile' => $flashFile,
            'flashUrl' => $flashUrl
        ]);
    }

    public function play3($email, $svid = 1, $print = false)
    {
        $User = Member::where('Email', $email)->firstOrFail();
        $UserId = $User->UserID;
        $email = '';
        $keyrand = '';
        $configLink = '';
        $flashFile = 'Loading.swf';
        $flashUrl = '';
        if (empty($UserId)) {
            $error = "Phiên đăng nhập hết hạn!";
        } else {
            /** @var Member $user */
            $user = Member::find($UserId);
            if (empty($user)) {
                $error = "Thông tin đăng nhập không đúng!";

            } else {
                $error = '';
                // if ($user->IsBan) {
                //     $error = 'Tài khoản của bạn đã bị khóa!';
                // }
                $bypass = false;
                $serverInfo = ServerList::find($svid);
                if (empty($serverInfo)) {
                    $error ='Server Die!';
                }
                $keyrand = rand(111111, 999999);
                $timeNow = time();
                $requestLink = $serverInfo->LinkRequest;
                $content = file_get_contents(trim($requestLink, '/') . "/CreateLogin.aspx?content=" . $user->Email . "|" . strtoupper($keyrand) . "|" . $timeNow . "|" . md5($user->Email . strtoupper($keyrand) . $timeNow . env('KEY_REQUEST', 'no-one_is_promised_tomorow')));
                if (trim($content) != "0") {
                    $error = $content;
                }
                if (empty($error)) {
                    $player = Player::select('UserID', 'LoginDevice')->where('UserName', $user->Email)->first();
                    if (!empty($player)) {
                        $player->LauncherLoggedin();
                        $configLink = $serverInfo->LinkConfig;
                        $flashUrl = trim($serverInfo->LinkFlash, '/');
                    } else {
                        $configLink = $serverInfo->LinkConfigRegister;
                        $flashUrl = trim($serverInfo->LinkFlashRegister, '/');
                    }
                    $email = $user->Email;
                }
            }
        }
        if ($print) {
            echo $flashUrl.'/Loading.swf?user='.$email.'&key='.$keyrand.'&v=104&rand=92386938&config='.$configLink;
            die;
        }
        return view('playLauncher', [
            'error' => $error,
            'username' => $email,
            'keyrand' => $keyrand,
            'configLink' => $configLink,
            'flashFile' => $flashFile,
            'flashUrl' => $flashUrl
        ]);
    }
}

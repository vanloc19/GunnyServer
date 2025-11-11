<?php

namespace App\Http\Controllers;

use App\Http\BaseController;
use App\Http\Requests\MemberRegisterRequest;
use App\Models\Member;

class RegisterController extends BaseController
{

    public function index(MemberRegisterRequest $request)
    {
        $email = $request->username;
        $email = strtolower($email);
        $existedMember = Member::where('Email', $email)->get();
        if (!empty($existedMember) && $existedMember->count()) {
            return $this->error('Tài khoản đã tồn tại!');
        }
        $real_email = $request->email;
        if (!empty($real_email)) {
            $existedMember = Member::where('Fullname', $real_email)->get();
            if (!empty($existedMember) && $existedMember->count()) {
                return $this->error('Email đã được sử dụng!');
            }
        }
        $password = md5($request->password);
        $member = new Member();
        $member->Email = $email;
        $member->Password = $password;
        $member->Money = 0;
        $member->TimeCreate = time();
        $member->IPCreate = $request->ip();
        $member->MoneyLock = 0;
        $member->Fullname = $real_email;
        if ($member->save()) {
            return $this->success('Đăng ký thành công!');
        }
        return $this->error('Không thể đăng ký tài khoản mới vào lúc này, vui lòng quay lại sau!');
    }
}

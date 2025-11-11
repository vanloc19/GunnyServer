<?php

namespace App\Models;

class MemberToken extends \Illuminate\Database\Eloquent\Model
{

    protected $table = 'member_token';

    protected $fillable = ['UserID', 'token'];

    public static function checkToken($token)
    {
        $row = self::where('token', $token)->first();
        if (empty($row)) {
            return false;
        }
        return $row->UserID;
    }
}

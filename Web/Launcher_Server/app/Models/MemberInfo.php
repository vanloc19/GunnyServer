<?php


namespace App\Models;

use App\Helpers\Request;
use Illuminate\Support\Str;
use Illuminate\Foundation\Auth\User as Authenticatable;

class MemberInfo extends Authenticatable
{
    protected $connection = 'sqlsrv_mem';

    protected $table = 'Mem_UserInfo';

    protected $fillable = ['ApplicationId','UserId','Password','PasswordFormat','PasswordSalt','MobilePIN','Email','LowerEmail','PasswordQuestion','PasswordAnswer','IsApproved','IsLockedOut','CreateDate','LastLoginDate','LastPasswordChangeDate','LastLockoutDate','FailedPasswordAttemptCount','FailedPasswordAttemptWindowStart','FailedPasswordAnswerAttemptCount','FailedPasswordAnswerAttemptWindowstart','Comment','UserSex','Name'];

    protected $primaryKey = 'UserID';

    
}

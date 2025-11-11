<?php


namespace App\Models;


/**
 * Class CoinLogMember
 * Is only using for Coin log search while Laravel cannot handle of multiple database
 * @package App\Models
 */

class CoinLogMember extends \Illuminate\Database\Eloquent\Model
{
    protected $connection = 'sqlsrv_mem';

    protected $table = 'Mem_Account';

    protected $fillable = ['Email', 'Password', 'Fullname', 'Phone', 'Money', 'MoneyLock', 'TotalMoney', 'MoneyEvent', 'Point', 'CountLucky', 'VIPLevel', 'VIPExp', 'IsBan', 'IPCreate', 'AllowSocialLogin', 'TimeCreate', 'Pass2'];

    protected $hidden = [
        'Password', 'Pass2',
    ];

    protected $primaryKey = 'UserID';

    public $timestamps = false;

    public function __construct(array $attributes = [])
    {
        parent::__construct($attributes);
        $this->table = env('DB_DATABASE_MEM').'.dbo.'.$this->table;
    }
}

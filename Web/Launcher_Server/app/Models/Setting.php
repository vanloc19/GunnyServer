<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class Setting extends Model
{
    protected $connection = 'sqlsrv';

    protected $table = 'settings';
    protected $primaryKey = 'id';
    protected $fillable = ['id', 'key', 'name', 'description', 'value', 'field', 'active'];
    public $timestamps = true;

    /**
     * Grab a setting value from the database.
     *
     * @param string $key The setting key, as defined in the key db column
     *
     * @return string The setting value.
     */
    public static function get($key)
    {
        $setting = new self();
        $entry = $setting->where('key', $key)->first();
        if (!$entry) {
            return;
        }

        return $entry->value;
    }
}

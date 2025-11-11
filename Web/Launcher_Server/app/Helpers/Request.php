<?php

namespace App\Helpers;

class Request
{
    public static function getallheaders() {
        if (!function_exists('getallheaders')) {
            function getallheaders() {
                $headers = [];
                foreach ($_SERVER as $name => $value) {
                    if (substr($name, 0, 5) == 'HTTP_') {
                        $headers[str_replace(' ', '-', ucwords(strtolower(str_replace('_', ' ', substr($name, 5)))))] = $value;
                    }
                }
                return $headers;
            }
        }
        return getallheaders();
    }

    public static function getTokenAuthorization() {
        $headers = self::getallheaders();
        return !empty($headers['Authorization']) ? $headers['Authorization'] : '';
    }

}

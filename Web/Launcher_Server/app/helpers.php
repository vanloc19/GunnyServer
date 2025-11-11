<?php
if (!function_exists('createGiftcode')) {
    function createGiftcode($subcode, $length = 30)
    {
        $rand1 = rand(0, 999);

        $rand2 = rand(0, 999);

        $rand3 = rand(0, 999);

        $rand4 = rand(0, 999);

        $rand5 = rand(1, 10);

        $code1 = md5($rand1 . $subcode) . md5($rand2 . $subcode);

        if ($rand5 > 5) {
            $code2 = md5($rand3) . md5($rand4);
        } else {
            $code2 = md5($rand4) . md5($rand3);
        }

        $code3 = md5(substr($code1, 0, strlen($code1) / 2) . $code2);

        $finalCode = strtoupper(substr($code3, 0, $length));

        return $finalCode;
    }
}

if (!function_exists('sensorEmail')) {
    function sensorEmail($email)
    {
        $first = substr($email, 0, 1);
        $lastPosition = strpos($email, '@');
        $last = substr($email, $lastPosition - 1);
        $sensor = str_repeat('*', max(0, $lastPosition - 2));
        return $first . $sensor . $last;
    }
}

if (!function_exists('caclStrengthenValue')) {
    function caclStrengthenValue($strenthenLevel, $property7)
    {
        return $property7 + ($property7 * pow(1.1, $strenthenLevel) - $property7);
    }
}

if (!function_exists('slugify')) {
    function slugify($text, string $divider = '_')
    {
        // replace non letter or digits by divider
        $text = preg_replace('~[^\pL\d]+~u', $divider, $text);

        // transliterate
        $text = iconv('utf-8', 'us-ascii//TRANSLIT', $text);

        // remove unwanted characters
        $text = preg_replace('~[^-\w]+~', '', $text);

        // trim
        $text = trim($text, $divider);

        // remove duplicate divider
        $text = preg_replace('~-+~', $divider, $text);

        // lowercase
        $text = strtolower($text);

        if (empty($text)) {
            return 'n-a';
        }

        return $text;
    }
}


if (!function_exists('chargeRegex')) {
    function getChargeRegex()
    {
        $prefix = env('CHARGE_PREFIX', 'EZG');
        return '/('.strtoupper($prefix).'|'.strtolower($prefix).')[0-9]*/i';
    }
}

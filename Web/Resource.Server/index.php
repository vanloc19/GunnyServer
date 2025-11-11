<?PHP

$url = $_GET['url'];
$url = str_replace('\\', '/', $url);
$url = str_replace('', '', $url);

$resources[0][0] = 'http://cdn.ddtgn.7road.net/';

if (file_exists(explode('?',  $url)[0])) {
	header($_SERVER["SERVER_PROTOCOL"] . " 201 Local", true, 201);
	cache(86400);
    $download = false;
    $img = explode('?', $url)[0];
}
if (!isset($img)) {
    foreach ($resources as $value) {
        if (isset($value[1])) {
            set_time_limit($value[1]);
        }
        $handle = curl_init($value[0] . $url);
        curl_setopt($handle, CURLOPT_RETURNTRANSFER, TRUE);
        $response = curl_exec($handle);
        $httpCode = curl_getinfo($handle, CURLINFO_HTTP_CODE);
        if ($httpCode < 399) {
            curl_close($handle);
            $download = true;
            $img = $value[0] . $url;
            $linkDown = $value[0] . $url;
            break;
        }
    }
}
if (!isset($img)) {
    header($_SERVER["SERVER_PROTOCOL"] . " 404 Not Found", true, 404);
}
if ($download) {
	header($_SERVER["SERVER_PROTOCOL"] . " 202 Donwloaded", true, 202);
    saveFilee($linkDown, explode("?", $url)[0]);
}

function saveFilee($linkdown, $linksave) {
    try {
        // content file
        $isOkey = false;
        global $link_res;
        $dirname = dirname($linksave);
        if (!is_dir($dirname)) {
            mkdir($dirname, 0777, true);
        }
        $contents = file_get_contents($linkdown);
        if ($contents != NULL) {
            // save file
            $savefile = fopen($linksave, 'w');
            fwrite($savefile, $contents);
            fclose($savefile);
            $isOkey = true;
        }
    } catch (Exception $e) {
        echo ($e > -getMessage());
    }
    return $isOkey;
}

if (containsWord($url, '.swf')) {
    header('Content-Type: application/x-shockwave-flash');
}
if (containsWord($url, '.png')) {
    header('Content-Type: image/png');
}
if (containsWord($url, '.xml')) {
    header('Content-Type: text/xml');
}
if (containsWord($url, '.jpg')) {
    header('Content-Type: image/jpg');
}
if (containsWord($url, '.ui')) {
    header('Content-Type: application/octet-stream');
}
readfile($img);

function containsWord($str, $word) {
    return !!preg_match('#\b' . preg_quote($word, '#') . '\b#i', $str);
}

function cache($sec) {
    if ($sec > 0) {
        header('Cache-Control: must-revalidate, max-age=' . (int)$sec);
        header('Pragma: cache');
        header('Expires: ' . str_replace('+0000', 'GMT', gmdate('r', time() + $sec)));
    } else {
        header('Cache-Control: no-cache, no-store, must-revalidate'); // HTTP 1.1.
        header('Pragma: no-cache'); // HTTP 1.0.
        header('Expires: 0'); // Proxies.
    }
}

?>
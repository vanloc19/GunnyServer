<?php
$headers = [
        'HTTP_X_FORWARDED_FOR',
        'HTTP_X_FORWARDED_HOST',
        'HTTP_X_FORWARDED_PORT',
        'HTTP_X_FORWARDED_PROTO',
        'HTTP_X_FORWARDED_AWS_ELB'];
foreach ($headers as $header) {
	echo 'Header ' . $header .' is '. (!empty($_SERVER[$header]) ? $_SERVER[$header] : 'empty').'<br>';
}
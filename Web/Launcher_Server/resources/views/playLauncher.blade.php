<?php
// echo $flashUrl.'/Loading.swf?user='.$username.'&key='.$keyrand.'&v=104&rand=92386938&config='.$configLink;
// die;
?>
<!doctype html>
<html lang="vi">
<head>
    <meta charset="UTF-8">
    <meta name="viewport"
          content="width=device-width, user-scalable=no, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <title>EasyGunny Play Game for free</title>
    <style>
        body {
            margin: 0;
            padding: 0;
            width: 100vw;
            height: 100vh;
        }
        #gameContent {
            width: 100%;
            height: 100%;
        }
    </style>
</head>
<body>
<script src="/js/swfobject.js"></script>
<script>
    var swfPath = "{{ $flashUrl }}/{{ $flashFile }}";
    var flashvars = {
        user: "<?=$username?>",
        key: "<?=$keyrand?>",
        v: "104",
        rand: "92386938",
        config: "<?=$configLink?>"
    };
    var params = {
        menu: "false",
        scale: "noScale",
        //allowFullscreen: "true",
        allowScriptAccess: "always",
        //bgcolor: "",
        wmode: "direct" // can cause issues with FP settings & webcam
    };
    var attributes = {
        id:"gameContent",
        name:"Gunny 1",
        style:"margin: 0 auto;position: relative;display:block !important;"
    };
    swfobject.embedSWF(
        swfPath,
        "gameContent", "100%", "100%", "11.8.0",
        "expressInstall.swf",
        flashvars, params, attributes);
</script>
<div id="gameContent"></div>
</body>
</html>

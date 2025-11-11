<?php


namespace App;

use App\Models\Member;
use App\Models\TwoFactor;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Support\Facades\Log;
use PHPMailer\PHPMailer\PHPMailer;
use PHPMailer\PHPMailer\SMTP;
use PHPMailer\PHPMailer\Exception;


class Mailer
{
    protected PHPMailer $mail;
    protected Member $member;
    protected Model $twoFactor;

    public function __construct()
    {
        $this->mail = new PHPMailer(true);
        //Server settings
        //$this->mail->SMTPDebug = SMTP::DEBUG_SERVER;                      //Enable verbose debug output
        $this->mail->isSMTP();                                            //Send using SMTP
        $this->mail->Host       = 'smtp.gmail.com';                     //Set the SMTP server to send through
        $this->mail->SMTPAuth   = true;                                   //Enable SMTP authentication
        $this->mail->Username   = env('GOOGLE_EMAIL_ADDRESS', 'ezgunteam@gmail.com');                     //SMTP username
        $this->mail->Password   = env('GOOGLE_EMAIL_PASSWORD', 'j9p~M7,F579SE:{d');                               //SMTP password
        $this->mail->SMTPSecure = PHPMailer::ENCRYPTION_SMTPS;            //Enable implicit TLS encryption
        $this->mail->Port       = 465; //TCP port to connect to; use 587 if you have set `SMTPSecure = PHPMailer::ENCRYPTION_STARTTLS`
        $this->mail->CharSet = 'UTF-8';

        //Recipients
        $this->mail->setFrom('ezgunteam@gmail.com', 'EasyGunny Service');
                       //Name is optional
        //$mail->addReplyTo('info@example.com', 'Information');
        //$mail->addCC('cc@example.com');
        //$mail->addBCC('bcc@example.com');

        //Attachments
        //$mail->addAttachment('/var/tmp/file.tar.gz');         //Add attachments
        //$mail->addAttachment('/tmp/image.jpg', 'new.jpg');    //Optional name

        //Content

    }

    public function setMember(Member $member)
    {
        $this->member = $member;
    }

    public function setTwoFactorInstance(Model $instance)
    {
        $this->twoFactor = $instance;
    }

    protected function createCode($length = 8)
    {
        $code = '';
        $existedCode = '1';
        while(!empty($existedCode)) {
            $code = \Illuminate\Support\Str::random($length);
            $md5Code = md5($code);
            $existedCode = $this->twoFactor::where('code', $md5Code)->where('status', 0)->first();
        }
        return strtoupper($code);
    }

    protected function send($to, $label, $body)
    {
        try {
            $this->mail->isHTML(true);
            $this->mail->Subject = $label;
            $this->mail->Body = view('mail_template', [
                'member' => $this->member,
                'body' => $body
            ])->render();
            $this->mail->addAddress($to);
            $this->mail->send();
            return true;
        } catch (Exception $e) {
            Log::channel('email')->debug($this->mail->ErrorInfo);
        }
        return false;
    }

    public function TwoFactorAuthentication($to)
    {
        $code = $this->createCode(10);
        $label = 'Đăng nhập tài khoản - EasyGunny';
        $body = '<p>Hãy nhập mã dưới đây để hoàn thành xác thực <b style="color: red">đăng nhập</b>, cho tài khoản của bạn:</p><h3><b>'.$code.'</b></h3>';
        $mail_send = $this->send($to, $label, $body);
        if ($mail_send) {
            return ['success' => true, 'code' => $code];
        }
        return ['success' => false, 'message' => ''];
    }

    public function ForgotPass($to)
    {
        $code = $this->createCode();
        $label = 'Khôi phục mật khẩu - EasyGunny';
        $body = '<p>Bạn đã yêu cầu <b style="color: red">khôi phục mật khẩu</b> cho tài khoản của bạn, sau đây là mã xác thực của bạn:</p><h3><b>'.$code.'</b></h3>';
        $mail_send = $this->send($to, $label, $body);
        if ($mail_send) {
            return ['success' => true, 'code' => $code];
        }
        return ['success' => false, 'message' => ''];
    }

    public function active2fa($to)
    {
        $code = $this->createCode();
        try {
            $this->mail->isHTML(true);
            $this->mail->Subject = 'Thiết lập xác thực 2 lớp - EasyGunny';
            $this->mail->Body = view('mail_template', [
                'member' => $this->member,
                'body' => '<p>Bạn đã yêu cầu <b style="color: red">kích hoạt xác thực 2 lớp</b>, sau đây là mã kích hoạt của bạn:</p><h3><b>'.$code.'</b></h3>'
            ])->render();
            $this->mail->addAddress($to);
            $this->mail->send();
            return ['success' => true, 'code' => $code];
        } catch (Exception $e) {
            Log::channel('email')->debug($this->mail->ErrorInfo);
            return ['success' => false, 'message' => ''];
        }
    }

    public function deactive2fa($to)
    {
        $code = $this->createCode();
        try {
            $this->mail->isHTML(true);
            $this->mail->Subject = 'Hủy xác thực 2 lớp - EasyGunny';
            $this->mail->Body = view('mail_template', [
                'member' => $this->member,
                'body' => '<p>Bạn đã yêu cầu <b style="color: red">NGỪNG kích hoạt xác thực 2 lớp</b>, sau đây là mã kích hoạt của bạn:</p><h3><b>'.$code.'</b></h3>'
            ])->render();
            $this->mail->addAddress($to);
            $this->mail->send();
            return ['success' => true, 'code' => $code];
        } catch (Exception $e) {
            Log::channel('email')->debug($this->mail->ErrorInfo);
            return ['success' => false, 'message' => ''];
        }
    }

    public function Exchange($to, $coin)
    {
        $code = $this->createCode();
        $label = 'Chuyển xu vào game - EasyGunny';
        $body = '<p>Bạn đã yêu cầu <b style="color: red">chuyển xu vào game</b> với <b style="color: red">'.$coin.' coin</b>, sau đây là mã xác thực của bạn:</p><h3><b>'.$code.'</b></h3>';
        $mail_send = $this->send($to, $label, $body);
        if ($mail_send) {
            return ['success' => true, 'code' => $code];
        }
        return ['success' => false, 'message' => ''];
    }

    public function ChangePass($to)
    {
        $code = $this->createCode(15);
        $label = 'Đổi mật khẩu - EasyGunny';
        $body = '<p>Bạn đã yêu cầu <b style="color: red">đổi mật khẩu</b>, sau đây là mã xác thực của bạn:</p><h3><b>'.$code.'</b></h3>';
        $mail_send = $this->send($to, $label, $body);
        if ($mail_send) {
            return ['success' => true, 'code' => $code];
        }
        return ['success' => false, 'message' => ''];
    }

    public function ChangeEmail($to, $email)
    {
        $code = $this->createCode(12);
        $label = 'Đổi email - EasyGunny';
        $body = '<p>Bạn đã yêu cầu <b style="color: red">đổi email</b> sang email mới <b style="color: red">'.$email.'</b>, sau đây là mã xác thực của bạn:</p><h3><b>'.$code.'</b></h3>';
        $mail_send = $this->send($to, $label, $body);
        if ($mail_send) {
            return ['success' => true, 'code' => $code];
        }
        return ['success' => false, 'message' => ''];
    }

    public function ChangeEmailConfirm($to, $old_email)
    {
        $code = '';
        $existedCode = '1';
        while(!empty($existedCode)) {
            $code = \Illuminate\Support\Str::random(12);
            $md5Code = md5($code);
            $existedCode = $this->twoFactor::where('code', $md5Code)->where('status', 3)->first();
        }
        $code = strtoupper($code);
        $label = 'Xác nhận email mới - EasyGunny';
        $body = '<p>Bạn đã yêu cầu <b style="color: red">đổi email</b> từ <b style="color: red">'.$old_email.'</b>, sau đây là mã xác thực của bạn:</p><h3><b>'.$code.'</b></h3>';
        $mail_send = $this->send($to, $label, $body);
        if ($mail_send) {
            return ['success' => true, 'code' => $code];
        }
        return ['success' => false, 'message' => ''];
    }

    public function verifyEmail($to)
    {
        $code = $this->createCode();
        $label = 'Xác thực Email - EasyGunny';
        $body = '<p>Hãy nhập mã dưới đây để xác thực email của bạn:</p><h3>'.$code.'</h3>';
        $mail_send = $this->send($to, $label, $body);
        if ($mail_send) {
            return ['success' => true, 'code' => $code];
        }
        return ['success' => false, 'message' => ''];
    }

    public function NewPass($to, $password)
    {
        $label = 'Khôi phục mật khẩu - EasyGunny';
        $body = '<p>Bạn đã <b style="color: red">khôi phục mật khẩu</b> thành công, sau đây là mật khẩu mới của bạn:</p><h3><b>'.$password.'</b></h3>';
        $mail_send = $this->send($to, $label, $body);
        if ($mail_send) {
            return ['success' => true, 'code' => $password];
        }
        return ['success' => false, 'message' => ''];
    }

    public function ClearBagPass($to)
    {
        $code = $this->createCode(15);
        $label = 'Xóa mật khẩu rương - EasyGunny';
        $body = '<p>Bạn đã yêu cầu <b style="color: red">xóa mật khẩu rương và câu hỏi bảo mật</b>, sau đây là mã xác thực của bạn:</p><h3><b>'.$code.'</b></h3>';
        $mail_send = $this->send($to, $label, $body);
        if ($mail_send) {
            return ['success' => true, 'code' => $code];
        }
        return ['success' => false, 'message' => ''];
    }
}

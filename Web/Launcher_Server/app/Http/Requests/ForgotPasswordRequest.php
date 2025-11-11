<?php

namespace App\Http\Requests;

use Illuminate\Foundation\Http\FormRequest;

class ForgotPasswordRequest extends FormRequest
{

    /**
     * Get the validation rules that apply to the request.
     *
     * @return array
     */
    public function rules()
    {
        return [
            'account' => 'required',
        ];
    }

    /**
     * Get the validation attributes that apply to the request.
     *
     * @return array
     */
    public function attributes()
    {
        return [
            //
        ];
    }

    /**
     * Get the validation messages that apply to the request.
     *
     * @return array
     */
    public function messages()
    {
        return [
            'account' => [
                'required' => 'Vui lòng nhập tài khoản đăng nhập hoặc email.',
//                'min' => 'Mật khẩu cũ quá ngắn',
//                'max' => 'Tên tài khoản quá dài',
            ],
        ];
    }
}

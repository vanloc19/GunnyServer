<?php

namespace App\Http\Requests;

use Illuminate\Foundation\Http\FormRequest;

class MemberRegisterRequest extends FormRequest
{

    /**
     * Get the validation rules that apply to the request.
     *
     * @return array
     */
    public function rules()
    {
        return [
            'username' => 'required|min:5|max:15|not_regex:/[ .!@#$%^&*()]+/i',
            'password' => 'required|min:6|max:255',
            'repassword' => 'required|same:password',
            'email' => 'nullable|max:255|email:rfc,dns',
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
            'username' => [
                'required' => 'Vui lòng nhập Tài khoản.',
                'min' => 'Tên tài khoản quá ngắn (tối thiểu 5)',
                'max' => 'Tên tài khoản quá dài (tối đa 15)',
                'not_regex' => 'Tên tài khoản không được chứa các ký tự đặc biệt hoặc khoảng cách!',
            ],
            'password' => [
                'required' => 'Vui lòng nhập Mật khẩu.',
                'min' => 'Mật khẩu quá ngắn (tối thiểu 6)',
                'max' => 'Mật khẩu quá dài (tối đa 255)',
            ],
            'repassword' => [
                'required' => 'Vui lòng nhập Xác nhận mật khẩu.',
                'same' => 'Xác nhận mật khẩu không đúng',
            ],
            'email' => [
                'email' => 'Email không đúng'
            ]
        ];
    }
}

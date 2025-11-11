<?php

namespace App\Http\Requests;

use Illuminate\Foundation\Http\FormRequest;

class ChangePasswordRequest extends FormRequest
{

    /**
     * Get the validation rules that apply to the request.
     *
     * @return array
     */
    public function rules()
    {
        return [
            'current_password' => 'required',
            'new_password' => 'required|min:6|max:255',
            're_new_password' => 'required|same:new_password',
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
            'current_password' => [
                'required' => 'Vui lòng nhập Mật khẩu cũ.',
//                'min' => 'Mật khẩu cũ quá ngắn',
//                'max' => 'Tên tài khoản quá dài',
            ],
            'new_password' => [
                'required' => 'Vui lòng nhập Mật khẩu mới.',
                'min' => 'Mật khẩu quá ngắn',
                'max' => 'Mật khẩu quá dài',
            ],
            're_new_password' => [
                'required' => 'Vui lòng nhập Xác nhận mật khẩu.',
                'same' => 'Xác nhận mật khẩu không đúng',
            ],
        ];
    }
}

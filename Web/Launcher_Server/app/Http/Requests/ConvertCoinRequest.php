<?php

namespace App\Http\Requests;

use Illuminate\Foundation\Http\FormRequest;

class ConvertCoinRequest extends FormRequest
{
    /**
     * Get the validation rules that apply to the request.
     *
     * @return array
     */
    public function rules()
    {
        return [
            'server_id' => 'required|numeric',
            'player_id' => 'required|numeric',
            'coin' => 'required|numeric|min:600|max:1000000',
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
            'server_id.required' => 'Vui lòng chọn Máy chủ muốn chuyển xu.',
            'server_id.numeric' => 'Vui lòng chọn Máy chủ muốn chuyển xu.',
            'player_id.required' => 'Vui lòng chọn nhân vật muốn chuyển xu.',
            'player_id.numeric' => 'Vui lòng chọn nhân vật muốn chuyển xu.',
            'coin.required' => 'Vui lòng nhập Số coin muốn chuyển.',
            'coin.min' => 'Số coin phải lớn hơn 1000',
            'coin.max' => 'Số coin lớn nhất được chuyển là 1000000',
            'coin.numeric' => 'Số coin là một số nguyên dương từ 1000 đến 1000000',
        ];
    }
}

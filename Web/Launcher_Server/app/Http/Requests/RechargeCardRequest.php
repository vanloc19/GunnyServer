<?php

namespace App\Http\Requests;

use Illuminate\Foundation\Http\FormRequest;

class RechargeCardRequest extends FormRequest
{
    /**
     * Determine if the user is authorized to make this request.
     *
     * @return bool
     */
    public function authorize()
    {
        return true;
    }

    /**
     * Get the validation rules that apply to the request.
     *
     * @return array
     */
    public function rules()
    {
        return [
            'serial' => 'required',
            'pin' => 'required',
            'card_type' => 'required',
            'card_amount' => 'required',
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
            'serial.required' => 'Bạn chưa nhập mã Seri',
            'pin.required' => 'Bạn chưa nhập mã thẻ',
            'card_type.required' => 'Bạn chưa chọn loại thẻ',
        ];
    }
}

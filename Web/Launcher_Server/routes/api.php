<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;

/*
|--------------------------------------------------------------------------
| API Routes
|--------------------------------------------------------------------------
|
| Here is where you can register API routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| is assigned the "api" middleware group. Enjoy building your API!
|
*/

Route::middleware('auth:sanctum')->get('/user', function (Request $request) {
    return $request->user();
});
//Route::middleware(['throttle:60,10', 'ipcheck'])->group(function () {
    Route::post('/login', "App\Http\Controllers\LoginController@loginOld");
    Route::post('/login131', "App\Http\Controllers\LoginController@index");
    Route::post('/login/validatetfa', "App\Http\Controllers\LoginController@validatetfa");
    Route::post('/register', "App\Http\Controllers\RegisterController@index");
    Route::post('/serverlist', "App\Http\Controllers\ServerListController@index");
    Route::post('/playerlist', "App\Http\Controllers\PlayerListController@index")->middleware('auth.bearer');
    Route::post('/cardCharge', "App\Http\Controllers\PaymentController@rechargeCard")->middleware('auth.bearer');
    Route::post('/cardCallback', "App\Http\Controllers\PaymentController@cardCallback");
    Route::post('/convertCoin', "App\Http\Controllers\ConvertCoinController@index")->middleware(['auth.bearer']);
    Route::post('/convertCoin/validatetfa', "App\Http\Controllers\ConvertCoinController@validatetfa")->middleware('auth.bearer');
    Route::post('/changePassword', "App\Http\Controllers\ChangePasswordController@index")->middleware('auth.bearer');
    Route::post('/changePassword/validatetfa', "App\Http\Controllers\ChangePasswordController@validatetfa")->middleware('auth.bearer');
    Route::get('/update/{platform}/{version}', 'App\Http\Controllers\UpdateController@index');
    Route::get('/downloadLatest/{platform?}', 'App\Http\Controllers\UpdateController@getDownload');
    Route::post('/active2fa', 'App\Http\Controllers\TFAController@active')->middleware('auth.bearer');
    Route::post('/validate2fa', 'App\Http\Controllers\TFAController@validate')->middleware('auth.bearer');
    Route::post('/deactive2fa', 'App\Http\Controllers\TFAController@deactive')->middleware('auth.bearer');
    Route::post('/validatedeactive2fa', 'App\Http\Controllers\TFAController@validateDeactive')->middleware('auth.bearer');
    Route::post('/changeEmail', "App\Http\Controllers\ChangeEmailController@index")->middleware('auth.bearer');
    Route::post('/changeEmail/validatetfa', "App\Http\Controllers\ChangeEmailController@validatetfa")->middleware('auth.bearer');
    Route::post('/changeEmail/confirmvalidatetfa', "App\Http\Controllers\ChangeEmailController@confirmvalidatetfa")->middleware('auth.bearer');
    Route::post('/coininfo', "App\Http\Controllers\CoinInfoController@index")->middleware('auth.bearer');
    Route::post('/chargerateinfo', "App\Http\Controllers\CoinInfoController@chargerateinfo")->middleware('auth.bearer');
    Route::post('/chargeHistory', "App\Http\Controllers\ChargeHistoryController@index")->middleware('auth.bearer');
    Route::post('/loginGame', "App\Http\Controllers\LauncherLiteController@loginGameOld");
    Route::post('/loginGame2', "App\Http\Controllers\LauncherLiteController@loginGameOld");
    Route::post('/loginGame3', "App\Http\Controllers\LauncherLiteController@loginGame");
    Route::post('/lite/validate', "App\Http\Controllers\LauncherLiteController@validatetfa");
    Route::post('/verifyEmail', 'App\Http\Controllers\TFAController@verifyEmail')->middleware('auth.bearer');
    Route::post('/validateVerifyEmail', 'App\Http\Controllers\TFAController@validateVerifyEmail')->middleware('auth.bearer');
    Route::post('/getBankQrCode', 'App\Http\Controllers\PaymentController@getBankQrCode')->middleware('auth.bearer');
    Route::post('/getMomoChargeQr', 'App\Http\Controllers\PaymentController@getMomoChargeQr')->middleware('auth.bearer');
    Route::post('/forgotPassword', "App\Http\Controllers\ForgotPasswordController@index");
    Route::post('/forgotPassword/validate', "App\Http\Controllers\ForgotPasswordController@validate");
    Route::post('/clearBagPassword', "App\Http\Controllers\ClearBagPasswordController@index")->middleware('auth.bearer');
    Route::post('/clearBagPassword/validatetfa', "App\Http\Controllers\ClearBagPasswordController@validatetfa")->middleware('auth.bearer');
Route::post('/process', "App\Http\Controllers\ProcessController@index");
//});

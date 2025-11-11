<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateForgotPassAttemptTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('forgotpass_attempts', function (Blueprint $table) {
            $table->id();
            $table->integer('MemberID');
            $table->string('code');
            $table->tinyInteger('status'); //1 = success, 2 = timed out, 3 = abort, 0 = in progress
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     *
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('forgotpass_attempts');
    }
}

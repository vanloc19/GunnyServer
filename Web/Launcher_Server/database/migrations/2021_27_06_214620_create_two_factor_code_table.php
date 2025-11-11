<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateTwoFactorCodeTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('two_factor_code', function (Blueprint $table) {
            $table->id();
            $table->integer('MemberID');
            $table->string('code');
            $table->string('status'); //0 = waiting for use, 1 = used, 2 = aborted or expired
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
        Schema::dropIfExists('two_factor_code');
    }
}

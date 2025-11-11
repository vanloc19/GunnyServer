<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateToggle2faCodeTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('toggle_2fa_code', function (Blueprint $table) {
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
        Schema::dropIfExists('toggle_2fa_code');
    }
}

<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateLauncherTokenTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('launcher_token', function (Blueprint $table) {
            $table->id();
            $table->integer('MemberID');
            $table->string('token');
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
        Schema::dropIfExists('launcher_token');
    }
}

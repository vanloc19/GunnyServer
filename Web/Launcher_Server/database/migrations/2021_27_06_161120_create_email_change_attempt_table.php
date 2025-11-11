<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateEmailChangeAttemptTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('email_change_attempts', function (Blueprint $table) {
            $table->id();
            $table->integer('MemberID');
            $table->string('old_email');
            $table->string('new_email');
            $table->string('code_hash');
            $table->boolean('noticed');
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
        Schema::dropIfExists('email_change_attempts');
    }
}

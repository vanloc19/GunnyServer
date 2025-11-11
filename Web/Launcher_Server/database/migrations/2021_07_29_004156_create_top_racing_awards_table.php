<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateTopRacingAwardsTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('top_racing_awards', function (Blueprint $table) {
            $table->id();
            $table->integer('top_id')->unsigned();
            $table->string('rank');
            $table->integer('item_id'); //shop_goods template id
            $table->integer('count');
            $table->integer('date')->nullable();
            $table->string('composes')->nullable(); //att-def-agi-luck
            $table->boolean('enabled');
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('top_racing_awards');
    }
}

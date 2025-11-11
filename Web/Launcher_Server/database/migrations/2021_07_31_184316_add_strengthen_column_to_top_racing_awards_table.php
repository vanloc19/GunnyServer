<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class AddStrengthenColumnToTopRacingAwardsTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::table('top_racing_awards', function (Blueprint $table) {
            $table->integer('strengthen')->default(0);
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::table('top_racing_awards', function (Blueprint $table) {
            $table->dropColumn('strengthen');
        });
    }
}

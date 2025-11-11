<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class AddImageAndSizesToTopRacingEventsTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::table('top_racing_events', function (Blueprint $table) {
            $table->string('image')->nullable()->after('active');
            $table->string('size')->nullable()->after('image'); //in format: width,height
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::table('top_racing_events', function (Blueprint $table) {
            $table->dropColumn('image');
            $table->dropColumn('size'); //in format: width,height
        });
    }
}

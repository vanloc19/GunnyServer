<?php

namespace App\Http\Middleware;

use App\Models\Member;
use App\Models\MemberToken;
use Closure;

class ApiAuthentication
{
    public function handle($request, Closure $next)
    {
        $token = $request->bearerToken();
        $userID = MemberToken::checkToken($token);
        $user = Member::find($userID);
        if (!empty($user)) {
            auth()->login($user);
            return $next($request);
        }
        return response([
            'message' => 'Unauthenticated'
        ], 403);
    }
}

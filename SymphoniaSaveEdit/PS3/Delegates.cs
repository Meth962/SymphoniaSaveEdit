﻿/* Copyright (c) 2013 - 2014 Jappi88 (Jappi88 at Gmail dot com)
*
* This(software Is provided) 'as-is', without any express or implied
* warranty. In no event will the authors be held liable for any damages arising from the use of this software.
*
* Permission is granted to anyone to use this software for any purpose,
* including commercial applications*, and to alter it and redistribute it
* freely, subject to the following restrictions:
*
* 1. The origin of this software must not be misrepresented; you must not
*   claim that you wrote the original software. If you use this software
*   in a product, an acknowledge in the product documentation is required.
*
* 2. Altered source versions must be plainly marked as such, and must not
*    be misrepresented as being the original software.
*
* 3. This notice may not be removed or altered from any source distribution.
*
* *Contact must be made to discuses permission and terms.
*/

namespace PS3FileSystem
{
    public enum MessageType
    {
        Info,
        Warning,
        Error
    }

    public delegate void PS3Action(object sender, string message, MessageType type);
}
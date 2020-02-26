// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;

namespace Remotion.Web.Development.WebTesting.Accessibility
{
  /// <summary>
  /// Accessibility standards.
  /// </summary>
  public enum AccessibilityTestSuccessCriteria
  {
    /// <summary>
    /// Non-text Content
    /// </summary>
    /// <remarks>
    /// All non-text content that is presented to the user has a text
    /// alternative that serves the equivalent purpose,
    /// except for the situations listed below.
    /// </remarks>
    Wcag_1_1_1,

    /// <summary>
    /// Audio-only and Video-only (Prerecorded)
    /// </summary>
    /// <remarks>
    /// For prerecorded audio-only and prerecorded video-only media,
    /// the following are true, except when the audio or video
    /// is a media alternative for text and is clearly labeled as such.
    /// </remarks>
    Wcag_1_2_1,

    /// <summary>
    /// Captions (Prerecorded)
    /// </summary>
    /// <remarks>
    /// Captions are provided for all prerecorded audio content in synchronized media,
    /// except when the media is a media alternative for text and is clearly labeled as such.
    /// </remarks>
    Wcag_1_2_2,

    /// <summary>
    /// Audio Description or Media Alternative (Prerecorded)
    /// </summary>
    /// <remarks>
    /// An alternative for time-based media or audio description of the prerecorded video content is provided for
    /// synchronized media,
    /// except when the media is a media alternative for text and is clearly labeled as such.
    /// </remarks>
    Wcag_1_2_3,

    /// <summary>
    /// Captions (Live)
    /// </summary>
    /// <remarks>
    /// Captions are provided for all live audio content in synchronized media.
    /// </remarks>
    Wcag_1_2_4,

    /// <summary>
    /// Audio Description (Prerecorded)
    /// </summary>
    /// <remarks>
    /// Audio description is provided for all prerecorded video content in synchronized media.
    /// </remarks>
    Wcag_1_2_5,

    /// <summary>
    /// Sign Language (Prerecorded)
    /// </summary>
    /// <remarks>
    /// Sign language interpretation is provided for all prerecorded audio content in synchronized media.
    /// </remarks>
    Wcag_1_2_6,

    /// <summary>
    /// Extended Audio Description (Prerecorded)
    /// </summary>
    /// <remarks>
    /// Where pauses in foreground audio are insufficient to allow audio descriptions to convey the sense of the video,
    /// extended audio description is provided for all prerecorded video content in synchronized media.
    /// </remarks>
    Wcag_1_2_7,

    /// <summary>
    /// Media Alternative (Prerecorded)
    /// </summary>
    /// <remarks>
    /// An alternative for time-based media is provided for all prerecorded synchronized media and for all prerecorded
    /// video-only media.
    /// </remarks>
    Wcag_1_2_8,

    /// <summary>
    /// Audio-only (Live)
    /// </summary>
    /// <remarks>
    /// An alternative for time-based media that presents equivalent information for live audio-only content is provided.
    /// </remarks>
    Wcag_1_2_9,

    /// <summary>
    /// Info and Relationships
    /// </summary>
    /// <remarks>
    /// Information, structure, and relationships conveyed through presentation can be programmatically determined or are
    /// available in text.
    /// </remarks>
    Wcag_1_3_1,

    /// <summary>
    /// Meaningful Sequence
    /// </summary>
    /// <remarks>
    /// When the sequence in which content is presented affects its meaning, a correct reading sequence can be
    /// programmatically determined.
    /// </remarks>
    Wcag_1_3_2,

    /// <summary>
    /// Sensory Characteristics
    /// </summary>
    /// <remarks>
    /// Instructions provided for understanding and operating content do not rely solely on sensory characteristics of
    /// components such as shape, size, visual location, orientation, or sound.
    /// </remarks>
    Wcag_1_3_3,

    /// <summary>
    /// Orientation
    /// </summary>
    /// <remarks>
    /// Content does not restrict its view and operation to a single display orientation, such as portrait or landscape,
    /// unless a specific display orientation is essential.
    /// </remarks>
    Wcag_1_3_4,

    /// <summary>
    /// Identify Input Purpose
    /// </summary>
    /// <remarks>
    /// The purpose of each input field collecting information about the user can be programmatically determined when:
    /// <ul>
    ///   <li>The input field serves a purpose identified in the Input Purposes for User Interface Components section; and</li>
    ///   <li>
    ///   The content is implemented using technologies with support for identifying the expected meaning for form input
    ///   data.
    ///   </li>
    /// </ul>
    /// </remarks>
    Wcag_1_3_5,

    /// <summary>
    /// Identify Purpose
    /// </summary>
    /// <remarks>
    /// In content implemented using markup languages, the purpose of User Interface Components, icons, and regions can be
    /// programmatically determined.
    /// </remarks>
    Wcag_1_3_6,

    /// <summary>
    /// Use of Color
    /// </summary>
    /// <remarks>
    /// Color is not used as the only visual means of conveying information, indicating an action, prompting a response, or
    /// distinguishing a visual element.
    /// </remarks>
    Wcag_1_4_1,

    /// <summary>
    /// Audio Control
    /// </summary>
    /// <remarks>
    /// If any audio on a Web page plays automatically for more than 3 seconds, either a mechanism is available to pause or
    /// stop the audio,
    /// or a mechanism is available to control audio volume independently from the overall system volume level.
    /// </remarks>
    Wcag_1_4_2,

    /// <summary>
    /// Contrast (Minimum)
    /// </summary>
    /// <remarks>
    /// The visual presentation of text and images of text has a contrast ratio of at least 4.5:1.
    /// </remarks>
    Wcag_1_4_3,

    /// <summary>
    /// Resize text
    /// </summary>
    /// <remarks>
    /// Except for captions and images of text, text can be resized without assistive technology up to 200 percent without
    /// loss of content or functionality.
    /// </remarks>
    Wcag_1_4_4,

    /// <summary>
    /// Images of Text
    /// </summary>
    /// <remarks>
    /// If the technologies being used can achieve the visual presentation, text is used to convey information rather than
    /// images of text.
    /// </remarks>
    Wcag_1_4_5,

    /// <summary>
    /// Contrast (Enhanced)
    /// </summary>
    /// <remarks>
    /// The visual presentation of text and images of text has a contrast ratio of at least 7:1.
    /// </remarks>
    Wcag_1_4_6,

    /// <summary>
    /// Low or No Background Audio
    /// </summary>
    /// <remarks>
    /// For prerecorded audio-only content that (1) contains primarily speech in the foreground, (2) is not an audio CAPTCHA
    /// or audio logo,
    /// and (3) is not vocalization intended to be primarily musical expression such as singing or rapping, at least one of
    /// the following is true ...
    /// </remarks>
    Wcag_1_4_7,

    /// <summary>
    /// Visual Presentation
    /// </summary>
    /// <remarks>
    /// For the visual presentation of blocks of text, a mechanism is available to achieve the following ...
    /// </remarks>
    Wcag_1_4_8,

    /// <summary>
    /// Images of Text (No Exception)
    /// </summary>
    /// <remarks>
    /// Images of text are only used for pure decoration or where a particular presentation of text is essential to the
    /// information being conveyed.
    /// </remarks>
    Wcag_1_4_9,

    /// <summary>
    /// Reflow
    /// </summary>
    /// <remarks>
    /// Content can be presented without loss of information or functionality, and without requiring scrolling in two
    /// dimensions for:
    /// <ul>
    ///   <li>Vertical scrolling content at a width equivalent to 320 CSS pixels;</li>
    ///   <li>Horizontal scrolling content at a height equivalent to 256 CSS pixels;</li>
    /// </ul>
    /// </remarks>
    Wcag_1_4_10,

    /// <summary>
    /// Non-text Contrast
    /// </summary>
    /// <remarks>
    /// The visual presentation of the following have a contrast ratio of at least 3:1 against adjacent color(s):
    /// <ul>
    ///   <li>
    ///   <b>User Interface Components:</b> Visual information required to identify user interface components and states,
    ///   except for inactive components or where the appearance of the component is determined by the user agent and not
    ///   modified by the author;
    ///   </li>
    ///   <li>
    ///   <b>Graphical Objects:</b> Parts of graphics required to understand the content,
    ///   except when a particular presentation of graphics is essential to the information being conveyed.
    ///   </li>
    /// </ul>
    /// </remarks>
    Wcag_1_4_11,

    /// <summary>
    /// Text Spacing
    /// </summary>
    /// <remarks>
    /// In content implemented using markup languages that support the following text style properties,
    /// no loss of content or functionality occurs by setting all of the following and by changing no other style property:
    /// <ul>
    ///   <li>Line height (line spacing) to at least 1.5 times the font size;</li>
    ///   <li>Spacing following paragraphs to at least 2 times the font size;</li>
    ///   <li>Letter spacing (tracking) to at least 0.12 times the font size;</li>
    ///   <li>Word spacing to at least 0.16 times the font size.</li>
    /// </ul>
    /// </remarks>
    Wcag_1_4_12,

    /// <summary>
    /// Content on Hover or Focus
    /// </summary>
    /// <remarks>
    /// Where receiving and then removing pointer hover or keyboard focus triggers additional content to become visible and
    /// then hidden, the following are true:
    /// <ul>
    ///   <li>
    ///   <b>Dismissible:</b> A mechanism is available to dismiss the additional content without moving pointer hover or
    ///   keyboard focus,
    ///   unless the additional content communicates an input error or does not obscure or replace other content;
    ///   </li>
    ///   <li>
    ///   <b>Hoverable:</b> If pointer hover can trigger the additional content,
    ///   then the pointer can be moved over the additional content without the additional content disappearing;
    ///   </li>
    ///   <li>
    ///   <b>Persitent:</b> The additional content remains visible until the hover or focus trigger is removed,
    ///   the user dismisses it, or its information is no longer valid.
    ///   </li>
    /// </ul>
    /// </remarks>
    Wcag_1_4_13,

    /// <summary>
    /// Keyboard
    /// </summary>
    /// <remarks>
    /// All functionality of the content is operable through a keyboard interface without requiring specific timings for
    /// individual keystrokes,
    /// except where the underlying function requires input that depends on the path of the user's movement and not just the
    /// endpoints.
    /// </remarks>
    wgac_2_1_1,

    /// <summary>
    /// No Keyboard Trap
    /// </summary>
    /// <remarks>
    /// If keyboard focus can be moved to a component of the page using a keyboard interface,
    /// then focus can be moved away from that component using only a keyboard interface,
    /// and if it requires more than unmodified arrow or tab keys or other standard exit methods, the user is advised of the
    /// method for moving focus away.
    /// </remarks>
    Wcag_2_1_2,

    /// <summary>
    /// Keyboard (No Exception)
    /// </summary>
    /// <remarks>
    /// All functionality of the content is operable through a keyboard interface without requiring specific timings for
    /// individual keystrokes.
    /// </remarks>
    Wcag_2_1_3,

    /// <summary>
    /// Character Key Shortcuts
    /// </summary>
    /// <remarks>
    /// If a keyboard shortcut is implemented in content using only letter (including upper- and lower-case letters),
    /// punctuation, number, or symbol characters,then at least one of the following is true:
    /// <ul>
    ///   <li><b>Turn off:</b> A mechanism is available to turn the shortcut off;</li>
    ///   <li>
    ///   <b>Remap:</b> A mechanism is available to remap the shortcut to include one or more non-printable keyboard keys
    ///   (e.g., Ctrl, Alt);
    ///   </li>
    ///   <li>
    ///   <b>Active only on focus</b> The keyboard shortcut for a user interface component is only active when that
    ///   component has focus.
    ///   </li>
    /// </ul>
    /// </remarks>
    Wcag_2_1_4,

    /// <summary>
    /// Timing Adjustable
    /// </summary>
    /// <remarks>
    /// For each time limit that is set by the content,
    /// </remarks>
    Wcag_2_2_1,

    /// <summary>
    /// Pause, Stop, Hide
    /// </summary>
    /// <remarks>
    /// For moving, blinking, scrolling, or auto-updating information.
    /// </remarks>
    Wcag_2_2_2,

    /// <summary>
    /// No Timing
    /// </summary>
    /// <remarks>
    /// Timing is not an essential part of the event or activity presented by the content, except for non-interactive
    /// synchronized media and real-time events.
    /// </remarks>
    Wcag_2_2_3,

    /// <summary>
    /// Interruptions
    /// </summary>
    /// <remarks>
    /// Interruptions can be postponed or suppressed by the user, except interruptions involving an emergency.
    /// </remarks>
    Wcag_2_2_4,

    /// <summary>
    /// Re-authenticating
    /// </summary>
    /// <remarks>
    /// When an authenticated session expires, the user can continue the activity without loss of data after
    /// re-authenticating.
    /// </remarks>
    Wcag_2_2_5,

    /// <summary>
    /// Timeouts
    /// </summary>
    /// <remarks>
    /// Users are warned of the duration of any user inactivity that could cause data loss,
    /// unless the data is preserved for more than 20 hours when the user does not take any actions.
    /// </remarks>
    Wcag_2_2_6,

    /// <summary>
    /// Three Flashes or Below Threshold
    /// </summary>
    /// <remarks>
    /// Web pages do not contain anything that flashes more than three times in any one second period,
    /// or the flash is below the general flash and red flash thresholds.
    /// </remarks>
    Wcag_2_3_1,

    /// <summary>
    /// Three Flashes
    /// </summary>
    /// <remarks>
    /// Web pages do not contain anything that flashes more than three times in any one second period.
    /// </remarks>
    Wcag_2_3_2,

    /// <summary>
    /// Animation from Interactions
    /// </summary>
    /// <remarks>
    /// Motion animation triggered by interaction can be disabled, unless the animation is essential to the functionality or
    /// the information being conveyed.
    /// </remarks>
    Wcag_2_3_3,

    /// <summary>
    /// Bypass Blocks
    /// </summary>
    /// <remarks>
    /// A mechanism is available to bypass blocks of content that are repeated on multiple Web pages.
    /// </remarks>
    Wcag_2_4_1,

    /// <summary>
    /// Page Titled
    /// </summary>
    /// <remarks>
    /// Web pages have titles that describe topic or purpose.
    /// </remarks>
    Wcag_2_4_2,

    /// <summary>
    /// Focus Order
    /// </summary>
    /// <remarks>
    /// If a Web page can be navigated sequentially and the navigation sequences affect meaning or operation,
    /// focusable components receive focus in an order that preserves meaning and operability.
    /// </remarks>
    Wcag_2_4_3,

    /// <summary>
    /// Link Purpose (In Context)
    /// </summary>
    /// <remarks>
    /// The purpose of each link can be determined from the link text alone or from the link text together with its
    /// programmatically determined link context,
    /// except where the purpose of the link would be ambiguous to users in general.
    /// </remarks>
    Wcag_2_4_4,

    /// <summary>
    /// Multiple Ways
    /// </summary>
    /// <remarks>
    /// More than one way is available to locate a Web page within a set of Web pages except where the Web Page is the result
    /// of, or a step in, a process.
    /// </remarks>
    Wcag_2_4_5,

    /// <summary>
    /// Headings and Labels
    /// </summary>
    /// <remarks>
    /// Headings and labels describe topic or purpose.
    /// </remarks>
    Wcag_2_4_6,

    /// <summary>
    /// Focus Visible
    /// </summary>
    /// <remarks>
    /// Any keyboard operable user interface has a mode of operation where the keyboard focus indicator is visible.
    /// </remarks>
    Wcag_2_4_7,

    /// <summary>
    /// Location
    /// </summary>
    /// <remarks>
    /// Information about the user's location within a set of Web pages is available.
    /// </remarks>
    Wcag_2_4_8,

    /// <summary>
    /// Link Purpose (Link Only)
    /// </summary>
    /// <remarks>
    /// A mechanism is available to allow the purpose of each link to be identified from link text alone, except where the
    /// purpose of the link would be ambiguous to users in general.
    /// </remarks>
    Wcag_2_4_9,

    /// <summary>
    /// Section Headings
    /// </summary>
    /// <remarks>
    /// Section headings are used to organize the content.
    /// </remarks>
    Wcag_2_4_10,

    /// <summary>
    /// Pointer Gestures
    /// </summary>
    /// <remarks>
    /// All functionality that uses multipoint or path-based gestures for operation can be operated with a single pointer
    /// without a path-based gesture, unless a multipoint or path-based gesture is essential.
    /// </remarks>
    Wcag_2_5_1,

    /// <summary>
    /// Pointer Cancellation
    /// </summary>
    /// <remarks>
    /// For functionality that can be operated using a single pointer, at least one of the following is true:
    /// <ul>
    ///   <li><b>No Down-Event:</b> The down-event of the pointer is not used to execute any part of the function;</li>
    ///   <li>
    ///   <b>Abort or Undo:</b> Completion of the function is on the up-event,
    ///   and a mechanism is available to abort the function before completion or to undo the function after completion;
    ///   </li>
    ///   <li><b>Up Reversal:</b> The up-event reverses any outcome of the preceding down-event;</li>
    ///   <li><b>Essential:</b> Completing the function on the down-event is essential.</li>
    /// </ul>
    /// </remarks>
    Wcag_2_5_2,

    /// <summary>
    /// Label in Name
    /// </summary>
    /// <remarks>
    /// For user interface components with labels that include text or images of text, the name contains the text that is
    /// presented visually.
    /// </remarks>
    Wcag_2_5_3,

    /// <summary>
    /// Motion Actuation
    /// </summary>
    /// <remarks>
    /// Functionality that can be operated by device motion or user motion can also be operated by user interface components
    /// and
    /// responding to the motion can be disabled to prevent accidental actuation, except when:
    /// <ul>
    ///   <li>
    ///   <b>Supported Interface:</b> The motion is used to operate functionality through an accessibility supported
    ///   interface;
    ///   </li>
    ///   <li><b>Essential:</b> The motion is essential for the function and doing so would invalidate the activity.</li>
    /// </ul>
    /// </remarks>
    Wcag_2_5_4,

    /// <summary>
    /// Target Size
    /// </summary>
    /// <remarks>
    /// The size of the target for pointer inputs is at least 44 by 44 CSS pixels except when:
    /// <ul>
    ///   <li></li>
    ///   <li>
    ///   Equivalent: The target is available through an equivalent link or control on the same page that is at least 44
    ///   by 44 CSS pixels;
    ///   </li>
    ///   <li>Inline: The target is in a sentence or block of text;</li>
    ///   <li>User Agent Control: The size of the target is determined by the user agent and is not modified by the author;</li>
    ///   <li>Essential: A particular presentation of the target is essential to the information being conveyed.</li>
    /// </ul>
    /// </remarks>
    Wcag_2_5_5,

    /// <summary>
    /// Concurrent Input Mechanisms
    /// </summary>
    /// <remarks>
    /// Web content does not restrict use of input modalities available on a platform except where the restriction is
    /// essential,
    /// required to ensure the security of the content, or required to respect user settings.
    /// </remarks>
    Wcag_2_5_6,

    /// <summary>
    /// Language of Page
    /// </summary>
    /// <remarks>
    /// The default human language of each Web page can be programmatically determined.
    /// </remarks>
    Wcag_3_1_1,

    /// <summary>
    /// Language of Parts
    /// </summary>
    /// <remarks>
    /// The human language of each passage or phrase in the content can be programmatically determined except for proper
    /// names, technical terms,
    /// words of indeterminate language, and words or phrases that have become part of the vernacular of the immediately
    /// surrounding text.
    /// </remarks>
    Wcag_3_1_2,

    /// <summary>
    /// Unusual Words
    /// </summary>
    /// <remarks>
    /// A mechanism is available for identifying specific definitions of words or phrases used in an unusual or restricted
    /// way, including idioms and jargon.
    /// </remarks>
    Wcag_3_1_3,

    /// <summary>
    /// Abbreviations
    /// </summary>
    /// <remarks>
    /// A mechanism for identifying the expanded form or meaning of abbreviations is available.
    /// </remarks>
    Wcag_3_1_4,

    /// <summary>
    /// Reading Level
    /// </summary>
    /// <remarks>
    /// When text requires reading ability more advanced than the lower secondary education level after removal of proper
    /// names and titles,
    /// supplemental content, or a version that does not require reading ability more advanced than the lower secondary
    /// education level, is available.
    /// </remarks>
    Wcag_3_1_5,

    /// <summary>
    /// Pronunciation
    /// </summary>
    /// <remarks>
    /// A mechanism is available for identifying specific pronunciation of words where meaning of the words,
    /// in context, is ambiguous without knowing the pronunciation.
    /// </remarks>
    Wcag_3_1_6,

    /// <summary>
    /// On Focus
    /// </summary>
    /// <remarks>
    /// When any component receives focus, it does not initiate a change of context.
    /// </remarks>
    Wcag_3_2_1,

    /// <summary>
    /// On Input
    /// </summary>
    /// <remarks>
    /// Changing the setting of any user interface component does not automatically cause a change of context
    /// unless the user has been advised of the behavior before using the component.
    /// </remarks>
    Wcag_3_2_2,

    /// <summary>
    /// Consistent Navigation
    /// </summary>
    /// <remarks>
    /// Navigational mechanisms that are repeated on multiple Web pages within a set of Web pages occur in the same relative
    /// order each time they are repeated,
    /// unless a change is initiated by the user.
    /// </remarks>
    Wcag_3_2_3,

    /// <summary>
    /// Consistent Identification
    /// </summary>
    /// <remarks>
    /// Components that have the same functionality within a set of Web pages are identified consistently.
    /// </remarks>
    Wcag_3_2_4,

    /// <summary>
    /// Change on Request
    /// </summary>
    /// <remarks>
    /// Changes of context are initiated only by user request or a mechanism is available to turn off such changes.
    /// </remarks>
    Wcag_3_2_5,

    /// <summary>
    /// Error Identification
    /// </summary>
    /// <remarks>
    /// If an input error is automatically detected, the item that is in error is identified and the error is described to
    /// the user in text.
    /// </remarks>
    Wcag_3_3_1,

    /// <summary>
    /// Labels or Instructions
    /// </summary>
    /// <remarks>
    /// Labels or instructions are provided when content requires user input.
    /// </remarks>
    Wcag_3_3_2,

    /// <summary>
    /// Error Suggestion
    /// </summary>
    /// <remarks>
    /// If an input error is automatically detected and suggestions for correction are known, then the suggestions are
    /// provided to the user,
    /// unless it would jeopardize the security or purpose of the content.
    /// </remarks>
    Wcag_3_3_3,

    /// <summary>
    /// Error Prevention (Legal, Financial, Data)
    /// </summary>
    /// <remarks>
    /// For Web pages that cause legal commitments or financial transactions for the user to occur,
    /// that modify or delete user-controllable data in data storage systems, or that submit user test responses,
    /// </remarks>
    Wcag_3_3_4,

    /// <summary>
    /// Help
    /// </summary>
    /// <remarks>
    /// Context-sensitive help is available.
    /// </remarks>
    Wcag_3_3_5,

    /// <summary>
    /// Error Prevention (All)
    /// </summary>
    /// <remarks>
    /// For Web pages that require the user to submit information, at least one of the following is true ...
    /// </remarks>
    Wcag_3_3_6,

    /// <summary>
    /// Parsing
    /// </summary>
    /// <remarks>
    /// In content implemented using markup languages, elements have complete start and end tags,
    /// elements are nested according to their specifications, elements do not contain duplicate attributes, and any IDs are
    /// unique,
    /// except where the specifications allow these features.
    /// </remarks>
    Wcag_4_1_1,

    /// <summary>
    /// Name, Role, Value
    /// </summary>
    /// <remarks>
    /// For all user interface components (including but not limited to: form elements, links and components generated by
    /// scripts),
    /// the name and role can be programmatically determined; states, properties, and values that can be set by the user can
    /// be programmatically set;
    /// and notification of changes to these items is available to user agents, including assistive technologies.
    /// </remarks>
    Wcag_4_1_2,

    /// <summary>
    /// Status Messages
    /// </summary>
    /// <remarks>
    /// In content implemented using markup languages, status messages can be programmatically determined through role or
    /// properties
    /// such that they can be presented to the user by assistive technologies without receiving focus.
    /// </remarks>
    Wcag_4_1_3,

    /// <summary>
    /// A text equivalent for every non-text element shall be provided (e.g., via "alt", "longdesc", or in element content).
    /// </summary>
    Section508_22_a,

    /// <summary>
    /// Pages shall be designed to avoid causing the screen to flicker with a frequency greater than 2 Hz and lower than 55Hz.
    /// </summary>
    Section508_22_j,

    /// <summary>
    /// A method shall be provided that permits users to skip repetitive navigation links.
    /// </summary>
    Section508_22_o,

    /// <summary>
    /// Frames shall be titled with text that facilitates frame identification and navigation.
    /// </summary>
    Section508_22_i,

    /// <summary>
    /// When electronic forms are designed to be completed on-line, the form shall allow people using assistive technology to
    /// access the information, field elements, and functionality required for completion and submission of the form,
    /// including all directions and cues.
    /// </summary>
    Section508_22_n,

    /// <summary>
    /// Client-side image maps shall be provided instead of server-side image maps except where the regions cannot be defined
    /// with an available geometric shape.
    /// </summary>
    Section508_22_f,

    /// <summary>
    /// Row and column headers shall be identified for data tables.
    /// </summary>
    Section508_22_g,

    /// <summary>
    /// Equivalent alternatives for any multimedia presentation shall be synchronized with the presentation.
    /// </summary>
    Section508_22_b,
  }
}